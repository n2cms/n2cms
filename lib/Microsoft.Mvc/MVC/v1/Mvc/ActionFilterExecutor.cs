namespace System.Web.Mvc {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class ActionFilterExecutor {

        private FilterContext _context;
        private Action _continuation;
        private List<ActionFilterAttribute> _filters;

        public ActionFilterExecutor(List<ActionFilterAttribute> filters, FilterContext context, Action continuation) {
            _filters = filters;
            _context = context;
            _continuation = continuation;
        }

        public void Execute() {
            IEnumerator<ActionFilterAttribute> enumerator = _filters.GetEnumerator();
            ExecuteRecursive(enumerator);
        }

        private FilterExecutedContext ExecuteRecursive(IEnumerator<ActionFilterAttribute> enumerator) {
            if (enumerator.MoveNext()) {
                ActionFilterAttribute filter = enumerator.Current;
                FilterExecutingContext preContext = new FilterExecutingContext(_context);
                filter.OnActionExecuting(preContext);
                if (preContext.Cancel) {
                    return new FilterExecutedContext(_context, null /* exception */);
                }

                bool wasError = false;
                FilterExecutedContext postContext = null;
                try {
                    postContext = ExecuteRecursive(enumerator);
                }
                catch (Exception ex) {
                    wasError = true;
                    postContext = new FilterExecutedContext(_context, ex);
                    filter.OnActionExecuted(postContext);
                    if (!postContext.ExceptionHandled) {
                        throw;
                    }
                }
                if (!wasError) {
                    filter.OnActionExecuted(postContext);
                }
                return postContext;
            }
            else {
                _continuation();
                return new FilterExecutedContext(_context, null /* exception */);
            }
        }

    }
}
