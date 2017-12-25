using easyUpdater.Core.Handling;
using easyUpdater.Interfaces;

namespace easyUpdater.Core.Management
{
    public partial class UpdateManager
    {
        public UpdateManager(IEUpdatable iEUpdatable = null)
        {
            Initialize();
            Updater.SetApplicationInfo(iEUpdatable);
        }

        public UpdateHandler Updater { get; set; }
        public ParseHandler Parser { get; set; }

        private void Initialize()
        {
            Updater = new UpdateHandler(this);
            Parser = new ParseHandler(this);
        }
    }
}
