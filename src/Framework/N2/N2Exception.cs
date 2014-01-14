#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using System;

namespace N2
{
    /// <summary>The base class for the N2 application exceptions.</summary>
    public class N2Exception : ApplicationException
    {
        /// <summary>Creates a new instance of the N2Exception, the base class for known N2 exceptions.</summary>
        /// <param name="message">The exception message</param>
        public N2Exception(string message) : base(message)
        {
        }

        /// <summary>Creates a new instance of the N2Exception, the base class for known N2 exceptions.</summary>
        /// <param name="messageFormat">The exception message format.</param>
        /// <param name="args">The exception message arguments.</param>
        public N2Exception(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        /// <summary>Creates a new instance of the N2Exception that encapsulates an underlying exception.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The underlying exception.</param>
        public N2Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
