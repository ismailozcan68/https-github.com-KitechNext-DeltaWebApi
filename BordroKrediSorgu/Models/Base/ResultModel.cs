using System;

namespace DeltaWebApi.Models.Base
{
    public class ResultModel
    {
        public bool IsSucceed { get; set; } = true;
        public string Message { get; set; }
        public object TempObject { get; set; }

        public ResultModel()
        {

        }
        public ResultModel(bool isSucceed, string message)
        {
            IsSucceed = isSucceed;
            Message = message;
        }

        public ResultModel(bool isSucceed, string message, object tempOject) : this(isSucceed, message)
        {
            TempObject = tempOject;
        }

    }
}