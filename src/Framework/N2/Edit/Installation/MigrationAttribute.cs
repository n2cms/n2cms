using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Installation
{
    /// <summary>
    /// Marks a class deriving from <see cref="AbstractMigration"/> as a service which is included in migrations.
    /// </summary>
    public class MigrationAttribute : ServiceAttribute
    {
        public MigrationAttribute()
            : base(typeof(AbstractMigration))
        {
        }
    }
}
