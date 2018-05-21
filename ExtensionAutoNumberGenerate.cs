using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace AutoincrementAccount
{
    /// 
    /// Получение количества записей, созданных в интервале дат. 
    /// 
    partial class AutoNumberGenerate
    {
        private DateTime Now
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
        private DateTime Begin
        {
            get
            {
                return new DateTime(Now.Year, Now.Month, 1);
            }
        }

        private DateTime End
        {
            get
            {
                return new DateTime(Now.Year, Now.Month + 1, 1).AddDays(-1);
            }
        }

        private bool FirstRowMonth(IOrganizationService service, string entityName)
        {
            var _Query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = new ColumnSet(true),
                Criteria =
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                       {
                           new ConditionExpression
                           ( "createdon",ConditionOperator.OnOrAfter,Begin),
                           new ConditionExpression
                           ( "createdon",ConditionOperator.OnOrBefore,End)
                       }
                    }
            };
            var _Entities = service.RetrieveMultiple(_Query);
            return _Entities.Entities.Count > 0;
        }
    }
}
