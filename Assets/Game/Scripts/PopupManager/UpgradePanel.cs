using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : BaseMenuPopup
{
    #region Inspector Variables
    public Text heroName;
    public Image powerImg;
    public Text powerDamage, powerSpeed;
    public Image skillImg;
    public Text skillDamage, skillCoolDown;
    public Image[] heroRank;
    public GameObject btnUnlock, btnUpgrade, btnUpgradeRank,btnPrevHero, btnNextHero;
    public Button btnEquip;
    public Text rankCost, unlockCost, upgradeCost;

    #endregion;

    #region Member Variables
    private int currentShip;
    private int upgradePrice, unlockPrice, rankPrice;
    #endregion;

    #region Unity Methods
   
    #endregion;

    #region Public Methods

    public void InitUI()
    {       
        ViewShip(currentShip);        
    }


    public override void Show()
    {
        currentShip = PlayerData.Instance.selectedShip;
        InitUI();
    }

    public void OnUnlockHero()
    {

    }

    public void OnUpgradeHero()
    {
        if(PlayerData.Instance.gold > upgradePrice)
        {

        }
        else
        {
            // Notify not enough gold
        }
    }

    public void OnEquipHero()
    {

    }

    public void OnUpgradeRank()
    {

    }

    public void OnPrevHero()
    {
        currentShip -= 1;
        if (currentShip < 0 || currentShip >= ShipContainer.Instance.ship.Length)
            return;
        ViewShip(currentShip);
    }
    public void OnNextHero()
    {
        currentShip += 1;
        if (currentShip < 0 || currentShip >= ShipContainer.Instance.ship.Length)
            return;
        ViewShip(currentShip);
    }
    #endregion;

    #region Private Methods
    private void ViewShip(int id)
    {
        if (currentShip == PlayerData.Instance.selectedShip)
            btnEquip.interactable = false;
        else
            btnEquip.interactable = true;
        btnPrevHero.SetActive(true);
        btnNextHero.SetActive(true);
        if (currentShip <= 0)
            btnPrevHero.SetActive(false);
        else if (currentShip >= (ShipContainer.Instance.ship.Length - 1))
            btnNextHero.SetActive(false);

        ShipContainer.Instance.ShowShip(id);
        ShipData ship = ShipDataManager.Instance.shipData[id];
        heroName.text = ship.shipName;
        if (ship.skillImg != null)
            skillImg.sprite = ship.skillImg;
        if (ship.bulletImg.Length > 0)
        {
            powerImg.sprite = ship.bulletImg[0];
        }
        ShipUpgradeData shipUpgrade = PlayerData.Instance.shipData[id];
        powerDamage.text = (ship.baseDamage +(shipUpgrade.powerLevel * 5)).ToString();
        powerSpeed.text = (ship.minSpeed + (shipUpgrade.powerLevel * (ship.maxSpeed - ship.minSpeed) / 50)).ToString();
        
        if (!PlayerData.Instance.shipData[id].unlocked)
        {
            btnUnlock.SetActive(true);
            btnUpgrade.SetActive(false);
            unlockPrice = ship.crystal;
            unlockCost.text = unlockPrice.ToString();
        }
        else
        {
            btnUnlock.SetActive(false);
            btnUpgrade.SetActive(true);
            upgradePrice = 500 + PlayerData.Instance.shipData[id].powerLevel * 500;
            upgradeCost.text = upgradePrice.ToString();
        }
    }
    #endregion;

}
