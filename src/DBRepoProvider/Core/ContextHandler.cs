using System.Data.Entity;

namespace DBRepoProvider.Core
{
    public class ContextHandler
    {
        private volatile static ContextHandler _instance;
        private readonly DbContext _context;

        public DbContext Context
        {
            get { return _context; }
        }

        private ContextHandler(DbContext context)
        {
            _context = context;
        }

        public static ContextHandler GetContext(DbContext context)
        {
            if (_instance == null)
                _instance = new ContextHandler(context);
            else
                if (_instance.Context != context)
                _instance = new ContextHandler(context);

            return _instance;
        }
    }
}
