namespace System.Web.Mvc {
    public class ExceptionInfo {
        public string PropertyName {
            get;
            set;
        }

        public object AttemptedValue {
            get;
            set;
        }

        public string ErrorMessage {
            get;
            set;
        }
    }
}
