using System;
using System.Globalization;
using System.Web.Mvc;

namespace RP.Util
{

    public class DateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //string theDate = controllerContext.HttpContext.Request.QueryString[bindingContext.ModelName];
            //CultureInfo culture = CultureInfo.InvariantCulture;
            //DateTime dt = new DateTime();
            ////CultureInfo.GetCultureInfo("en-GB")
            //bool success = DateTime.TryParse(theDate, culture, DateTimeStyles.None, out dt);
            //if (success)
            //{
            //    return dt;
            //}
            //else
            //{
            //    return null;
            //    // Return an appropriate default
            //}

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            try
            {
                var date = value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
                return date;
            }
            catch (System.Exception)
            {
                return null;
            }

        }
    }
}