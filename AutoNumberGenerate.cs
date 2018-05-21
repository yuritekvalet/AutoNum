using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace AutoincrementAccount
{
    /// 
    /// Получение значения сущности отвечающей за автонумерацию. 
    ///Считывания ключевого поля,обработки логики, обновление сущностей.
    /// 
    partial class AutoNumberGenerate
    {
        public void GenerateAutoNumber(IOrganizationService service, Entity entity, string entityName, string fieldName)
        {
            string autonumberEntity = "ses_autonumberconfiguration";
            try
            {

                var query = new QueryByAttribute(autonumberEntity)
                {
                    ColumnSet = new ColumnSet("ac_name", "ac_currentvalue", "ac_numberofdigits", "ac_startingvalue")
                };
                query.Attributes.AddRange("ac_name");
                query.Values.AddRange(entityName);


                var retrieved = service.RetrieveMultiple(query);


                int currentval = 0;
                int numdigits = 0;
                int startvalue = 0;


                var entityObj = new Entity(autonumberEntity);


                foreach (var c in retrieved.Entities)
                {
                    entityObj.Id = (Guid)c.Attributes["ac_autonumberconfigurationid"]; ;
                    currentval = (int)c.Attributes["ac_currentvalue"];
                    numdigits = (int)c.Attributes["ac_numberofdigits"];
                    startvalue = (int)c.Attributes["ac_startingvalue"];
                }

                int value = Convert.ToInt32(currentval) + startvalue;
                //Реализации логики обнуления порядкового номера каждый месяц. 
                //Если запись является первой в текущем месяце то сбрасываем в 1, 
                //если это не первая запись за текущий месяц то осуществляем приращение счетчика.
                currentval = FirstRowMonth(service, entityName) ? currentval + 1 : 1;
                entityObj.Attributes.Add("ac_currentvalue", currentval);
                service.Update(entityObj);
                entity.Attributes[fieldName] = $"{Now:yyMM}-{value: D6}-SL";
                service.Update(entity);
            }
            catch (Exception)
            {
                return;
            }
        }


    }
}
