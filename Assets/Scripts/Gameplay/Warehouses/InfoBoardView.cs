using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace Gameplay.Warehouses
{
    public class InfoBoardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _inputWarehouseText;
        [SerializeField] private TMP_Text _outputWarehouseText;
        [SerializeField] private TMP_Text _statusManufactureText;

        private readonly string _inputWarehouseTemplate = "Input warehouse:{0}";
        private readonly string _outputWarehouseTemplate = "Output warehouse:{0}";
        private readonly string _manufactureStatusTemplate = "Manufacture status:{0}";
        private readonly string[] _manufactureStatus = {"waiting", "busy"};
        
        public void UpdateInfoBoard(int inputWarehouse, int outputWarehouse, bool statusManufacture)
        {
            _inputWarehouseText.text = string.Format(_inputWarehouseTemplate, inputWarehouse);
            _outputWarehouseText.text = string.Format(_outputWarehouseTemplate, outputWarehouse);
            _statusManufactureText.text = string.Format(_manufactureStatusTemplate, _manufactureStatus[Convert.ToInt32(statusManufacture)]);
        }

        #region Factory

        public class Factory : PlaceholderFactory<InfoBoardView>
        {
        }

        #endregion
    }

    public class InfoBoard
    {
        private readonly InfoBoardView _view;
        private IManufactureWarehouse _warehouse;
        private bool _statusManufacture;
        private IManufacture _manufacture;

        public InfoBoard(
            Transform parent, 
            InfoBoardView.Factory viewFactory)
        {
            _view = viewFactory.Create();
            _view.transform.SetParent(parent, false);
        }

        public void Initialize(IManufacture manufacture, IManufactureWarehouse warehouse)
        {
            _manufacture = manufacture;
            _warehouse = warehouse;
            
            UpdateInfoBoard();
            
            // _manufacture.ProgressChangedEvent += ManufactureOnProgressChangedHandler;
            //
            // _warehouse.Input.ResourceAddedEvent += ChangeResources;
            // _warehouse.Input.ResourceRemovedEvent += InputResourceRemovedHandler;
            //
            // _warehouse.Output.ResourceAddedEvent += OutputResourceAddedHandler;
            // _warehouse.Output.ResourceRemovedEvent += ChangeResources;
        }

        public void Dispose()
        {
            _manufacture.ProgressChangedEvent -= ManufactureOnProgressChangedHandler;
            
            // _warehouse.Input.ResourceAddedEvent -= ChangeResources;
            // _warehouse.Input.ResourceRemovedEvent -= InputResourceRemovedHandler;
            //
            // _warehouse.Output.ResourceAddedEvent -= OutputResourceAddedHandler;
            // _warehouse.Output.ResourceRemovedEvent -= ChangeResources;

            _statusManufacture = false;
        }
        
        private void ManufactureOnProgressChangedHandler(bool status)
        {
            _statusManufacture = status;
            UpdateInfoBoard();
        }

        private void OutputResourceAddedHandler(IResource _)
        {
            UpdateInfoBoard();
        }

        private void InputResourceRemovedHandler(IResource _)
        {
            UpdateInfoBoard();
        }

        private void ChangeResources(IResource _) => UpdateInfoBoard();

        private void UpdateInfoBoard()
        {
            // _view.UpdateInfoBoard(
            //     _warehouse.Input.StoredResources.Sum(x => x.Value.Amount),
            //     _warehouse.Output.StoredResources.Sum(x => x.Value.Amount),
            //     _statusManufacture);
            
            //_storedResources.Sum(x => x.Value)
        }

        public void Enabled(bool enabled)
        {
            _view.gameObject.SetActive(enabled);
        }

        #region Factory

        public class Factory : PlaceholderFactory<Transform, InfoBoard>
        {
        }

        #endregion
    }
}