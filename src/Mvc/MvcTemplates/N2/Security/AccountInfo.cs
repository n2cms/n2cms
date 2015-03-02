using System;
using System.Collections.Generic;
using System.Linq;
using N2.Engine;
using N2.Plugin;

namespace N2.Security
{
    /// <summary>
    /// Describes an account
    /// <seealso cref="AccountManager.GetUsers"/>
    /// </summary>
    public interface IAccountInfo
    {
        /// <summary> Username uniquely identifies the account </summary>
        string UserName { get; }

        string Email { get; }

        string Comment { get; }

        bool IsOnline { get; }

        bool IsLockedOut { get; }

        bool IsApproved { get; }

        DateTime CreationDate { get; }

        DateTime LastLoginDate { get; }

        DateTime LastLockoutDate { get; }
    }


    /// <summary> Utility implementation: an account </summary>
    public class AccountInfo : IAccountInfo
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public bool IsOnline { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
    }

}