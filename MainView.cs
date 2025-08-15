namespace MESHelper
{
    public partial class MainView : Form
    {
        public MainView()
        {
            if (MESHelperState.Instance != null) MESHelperState.Instance.MainViewInstance = this;
            InitializeComponent();       
        }
    }
}
