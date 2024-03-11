using TonkoGames.Controllers.Core;
using Models.DataModels;
using Models.Lobby;

namespace Models.Controllers
{
    public interface ILobbyModels
    {
        ICoinFarmerModel CoinFarmerModel { get; }
    }
    public interface ILobbyModelsRoot : ILobbyModels
    {
        void Init();
    }
    public class LobbyModels : ILobbyModelsRoot
    {
        private CoinFarmerModel _coinFarmerModel;

        public ICoinFarmerModel CoinFarmerModel => _coinFarmerModel;
        
        public LobbyModels(IDataCentralService dataCentralService, ConfigManager configManager)
        {
            _coinFarmerModel = new CoinFarmerModel(dataCentralService, configManager);
        }

        public void Init()
        {
            _coinFarmerModel.Init();
        }
    }
}
