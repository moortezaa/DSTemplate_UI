using DSTemplate_UI.Interfaces;
using DSTemplate_UI.Services;
using DSTemplate_UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSTemplate_UI.Business
{
    internal class DSTableManager : IDSTableManager
    {
        private readonly ViewRendererService _viewRendererService;

        public DSTableManager(ViewRendererService viewRendererService)
        {
            _viewRendererService = viewRendererService;
        }

        public async Task<IQueryable<T>> DoSFP<T>(IQueryable<T> datas, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10)
        {
            if (sortDesending != null && sortPropertyName != null)
            {
                //TODO: modify sort to support nested properties
                var arg = Expression.Parameter(typeof(T));
                var prop = Expression.Property(arg, sortPropertyName);
                var type = typeof(T).GetProperties().Where(p => p.Name == sortPropertyName).FirstOrDefault();
                var lambda = Expression.Lambda(prop, arg);
                MethodInfo method = null;
                if (sortDesending.Value)
                {
                    method = typeof(Queryable).GetMethods()
                      .Where(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                      .Single();
                }
                else
                {
                    method = typeof(Queryable).GetMethods()
                      .Where(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                      .Single();
                }
                MethodInfo orderMethod = method.MakeGenericMethod(typeof(T), type.PropertyType);
                datas = (IQueryable<T>)orderMethod.Invoke(null, new object[] { datas, lambda });
            }
            if (filters != null)
            {
                datas = ApplyFilter(datas, filters);
            }
            datas = datas.Skip((page - 1) * rowsPerPage).Take(rowsPerPage);
            return datas;
        }

        public async Task<string> RenderRow<T>(T data, IEnumerable<KeyValuePair<string, object>> viewData, string customRowView = null)
        {
            return await _viewRendererService.RenderViewToStringAsync(customRowView ?? "DSTable/Row", data, viewData);
        }

        public async Task<string> RenderRow<T>(T data, ViewDataDictionary viewData = null, string customRowView = null)
        {
            return await _viewRendererService.RenderViewToStringAsync(customRowView ?? "DSTable/Row", data, viewData);
        }

        public async Task<JsonResult> Json(IEnumerable<string> rows, string tableName)
        {
            return new JsonResult(new { rows, tableName });
        }

        public async Task<int> CountData<T>(IQueryable<T> datas, string filters)
        {
            if (filters != null)
            {
                datas = ApplyFilter(datas, filters);
            }
            return datas.Count();
        }

        public async Task<JsonResult> Json(int dataCount, string tableName)
        {
            return new JsonResult(new { dataCount, tableName });
        }

        private static IQueryable<T> ApplyFilter<T>(IQueryable<T> datas, string filters)
        {
            var filtersObject = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
            foreach (var filter in filtersObject)
            {
                var propNames = filter.PropertyName.Split('.');
                var MemberInfoes = new List<MemberInfo>();

                MemberInfo MemberInfo = typeof(T);
                MemberInfoes.Add(MemberInfo);
                if (propNames.Count() > 1)
                {
                    foreach (var propName in propNames)
                    {
                        if (MemberInfo is Type t)
                        {
                            //check if the type is an enumerable or a drived class of IEnumerable
                            if (t.IsGenericType && t.GetInterfaces().Any(i => i.Name.Contains("Enumerable")))
                            {
                                //get the type of the enumerable
                                var enumerableType = t.GetGenericArguments().First();
                                //get the property of the enumerable
                                var enumerableProperty = enumerableType.GetProperty(propName);
                                MemberInfo = enumerableProperty;
                            }
                            else
                            {
                                MemberInfo = t.GetProperty(propName);
                            }
                            //add property type to begining of list
                            MemberInfoes.Add(MemberInfo);
                        }
                        else if (MemberInfo is PropertyInfo p)
                        {
                            //check if the type is an enumerable or a drived class of IEnumerable
                            if (p.PropertyType.IsGenericType && p.PropertyType.GetInterfaces().Any(i => i.Name.Contains("Enumerable")))
                            {
                                //get the type of the enumerable
                                var enumerableType = p.PropertyType.GetGenericArguments().First();
                                //get the property of the enumerable
                                var enumerableProperty = enumerableType.GetProperty(propName);
                                MemberInfo = enumerableProperty;
                            }
                            else
                            {
                                MemberInfo = p.PropertyType.GetProperty(propName);
                            }
                            //add property type to begining of list
                            MemberInfoes.Add(MemberInfo);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    MemberInfo = typeof(T).GetProperty(filter.PropertyName);
                    MemberInfoes.Add(MemberInfo);
                }

                var filterExpretion = CreateFilterlambda(MemberInfoes, filter);

                if (filterExpretion != null)
                {
                    var method = typeof(Queryable).GetMethods()
                        .Where(m => m.Name == nameof(Queryable.Where) && m.GetParameters().Length == 2)
                        .First();
                    MethodInfo whereMethod = method.MakeGenericMethod(typeof(T));
                    datas = (IQueryable<T>)whereMethod.Invoke(null, new object[] { datas, filterExpretion });
                }
            }

            return datas;
        }

        private static LambdaExpression CreateFilterlambda(List<MemberInfo> members, DSFilterViewModel filterModel)
        {
            var member = members.First();
            var property = members.Skip(1).First();

            var memberType = member as Type;
            if (memberType == null)
            {
                memberType = ((PropertyInfo)member).PropertyType;
            }

            var lastPropertyType = property as Type;
            if (lastPropertyType == null)
            {
                lastPropertyType = ((PropertyInfo)property).PropertyType;
            }

            var parameter = Expression.Parameter(memberType, "x");
            var propertyExpression = Expression.Property(parameter, property.Name); // member.property
            for (int i = 1; i < members.Count - 1; i++)
            {
                if (i + 1 <= members.Count)
                {
                    var nextMember = members[i + 1];
                    var nextMemberType = nextMember as Type;
                    if (nextMemberType == null)
                    {
                        nextMemberType = ((PropertyInfo)nextMember).PropertyType;
                    }
                    //check if the type is an enumerable
                    if (nextMemberType.IsGenericType && nextMemberType.GetInterfaces().Any(i => i.Name.Contains("Enumerable")))
                    {
                        propertyExpression = Expression.Property(propertyExpression, nextMember.Name);

                        //get the type of the enumerable
                        var enumerableType = nextMemberType.GetGenericArguments().First();

                        var newMembers = new List<MemberInfo> { enumerableType };
                        newMembers.AddRange(members.Skip(i + 2));

                        var innerlambda = CreateFilterlambda(newMembers, filterModel);
                        if (innerlambda == null)
                        {
                            return null;
                        }
                        var anyMethod = typeof(Enumerable).GetMethods()
                            .Where(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
                            .First();
                        var anyMethodGeneric = anyMethod.MakeGenericMethod(enumerableType);
                        var anyCall = Expression.Call(anyMethodGeneric, propertyExpression, innerlambda);
                        return Expression.Lambda(anyCall, parameter);
                    }
                    else if (nextMemberType.IsClass)
                    {
                        propertyExpression = Expression.Property(propertyExpression, nextMember.Name);
                    }
                    lastPropertyType = nextMemberType;
                }
                else
                {
                    lastPropertyType = ((PropertyInfo)members[i]).PropertyType;
                    break;
                }
            }
            //we have the property now
            if (filterModel.FilterTerm != null)
            {
                if (lastPropertyType == typeof(string))
                {
                    var containsMethod = typeof(string).GetMethods()
                        .Where(m => m.Name == nameof(string.Contains))
                        .First();
                    var contains = Expression.Call(propertyExpression, containsMethod, new[] { Expression.Constant(filterModel.FilterTerm) }); // m.Name.Contains("filter.FilterTerm")
                    return Expression.Lambda(contains, parameter); // m => m.Name.Contains("filter.FilterTerm")
                }
                else if (lastPropertyType == typeof(bool))
                {
                    var boolValue = bool.Parse(filterModel.FilterTerm);
                    var equals = Expression.Equal(propertyExpression, Expression.Constant(boolValue)); // m.Name == filter.FilterTerm
                    return Expression.Lambda(equals, parameter);  // m => m.Name == filter.FilterTerm
                }
                else if (lastPropertyType.IsEnum)
                {
                    var enumValue = Enum.Parse(lastPropertyType, filterModel.FilterTerm);
                    var equals = Expression.Equal(propertyExpression, Expression.Constant(enumValue)); // m.Name == filter.FilterTerm
                    return Expression.Lambda(equals, parameter); // m => m.Name == filter.FilterTerm
                }
                else if (new Type[] { typeof(int), typeof(long), typeof(decimal), typeof(float) }.Contains(lastPropertyType))
                {
                    object value = null;
                    if (lastPropertyType == typeof(int))
                    {
                        value = int.Parse(filterModel.FilterTerm);
                    }
                    else if (lastPropertyType == typeof(long))
                    {
                        value = long.Parse(filterModel.FilterTerm);
                    }
                    else if (lastPropertyType == typeof(decimal))
                    {
                        value = decimal.Parse(filterModel.FilterTerm);
                    }
                    else if (lastPropertyType == typeof(float))
                    {
                        value = float.Parse(filterModel.FilterTerm);
                    }
                    var equals = Expression.Equal(propertyExpression, Expression.Constant(value)); // m.Name == filter.FilterTerm
                    return Expression.Lambda(equals, parameter); // m => m.Name == filter.FilterTerm
                }
            }
            else if (lastPropertyType == typeof(DateTime))
            {
                var afterStart = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(DateTime.Parse(filterModel.FilterTerms.From))); // m.Name >= filter.FilterTerms.From
                var beforEnd = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(DateTime.Parse(filterModel.FilterTerms.To))); // m.Name <= filter.FilterTerms.To
                var and = Expression.AndAlso(afterStart, beforEnd); // m.Name >= filter.FilterTerms.From && m.Name <= filter.FilterTerms.To
                return Expression.Lambda(and, parameter); // m => m.Name >= filter.FilterTerms.From && m.Name <= filter.FilterTerms.To
            }
            return null;
        }
    }
}
