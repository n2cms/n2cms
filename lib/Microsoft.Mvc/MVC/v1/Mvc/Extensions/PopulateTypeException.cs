namespace System.Web.Mvc {
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class PopulateTypeException : Exception {
        private Collection<ExceptionInfo> _loadExceptions = new Collection<ExceptionInfo>();

        public PopulateTypeException() {
        }

        public PopulateTypeException(string message)
            : base(message) {
        }

        public PopulateTypeException(string message, Exception inner)
            : base(message, inner) {
        }

        protected PopulateTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need to allow for population of this list during as Exceptions are handled in user code."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called as needed when Exception is handled in User Code")]
        public Collection<ExceptionInfo> LoadExceptions {
            get {
                return _loadExceptions;
            }
            set {
                _loadExceptions = value;
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }
    }
}
