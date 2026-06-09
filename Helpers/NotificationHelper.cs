using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DoAnCoSo.Helpers
{
    public static class NotificationHelper
    {
        public static void SetSuccessMessage(this ITempDataDictionary tempData, string message)
        {
            tempData["AlertMessage"] = message;
            tempData["AlertType"] = "success";
        }

        public static void SetErrorMessage(this ITempDataDictionary tempData, string message)
        {
            tempData["AlertMessage"] = message;
            tempData["AlertType"] = "danger";
        }

        public static void SetWarningMessage(this ITempDataDictionary tempData, string message)
        {
            tempData["AlertMessage"] = message;
            tempData["AlertType"] = "warning";
        }
    }
}