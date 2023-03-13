using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.DataAccess
{
    public class DatabaseContextFactory 
    {
        private readonly Action<DbContextOptionsBuilder> _configure;
        public DatabaseContextFactory(Action<DbContextOptionsBuilder> configure)
        {
            _configure = configure;
        }

        public DatabaseContext CreateContext()
        {
            DbContextOptionsBuilder<DatabaseContext> options = new();
            _configure(options);
            return new DatabaseContext(options.Options);
        }
    }
}
