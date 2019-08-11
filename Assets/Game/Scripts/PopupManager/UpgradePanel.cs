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
    public Image imgCrystal;
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
        if(PlayerData.Instance.crystal >= unlockPrice)
        {
            PlayerData.Instance.shipData[currentShip].unlocked = true;
            PlayerData.Instance.SaveShipData(currentShip);
            ViewShip(currentShip);
        }
        else
        {
            // Notify not enough crystal
        }
    }

    public void OnUpgradeHero()
    {
        if(PlayerData.Instance.gold >= upgradePrice)
        {
            PlayerData.Instance.gold -= upgradePrice;
            PlayerData.Instance.shipData[currentShip].powerLevel++ ;
            PlayerData.Instance.SaveAllData();
            ViewShip(currentShip);
        }
        else
        {
            // Notify not enough gold
        }
    }

    public void OnEquipHero()
    {
        if (PlayerData.Instance.shipData[currentShip].unlocked)
        {
            PlayerData.Instance.selectedShip = currentShip;
            PlayerData.Instance.SaveAllData();
            btnEquip.interactable = false;
        }
    }

    public void OnUpgradeRank()
    {
        if (PlayerData.Instance.crystal >= rankPrice)
        {
            PlayerData.Instance.crystal -= rankPrice;
            PlayerData.Instance.shipData[currentShip].rankLevel++;
            PlayerData.Instance.SaveAllData();
            ViewShip(currentShip);
        }
        else
        {
            // Notify not enough gold
        }
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
        float damage = (ship.baseDamage + (shipUpgrade.powerLevel * 5));
        float speed = (ship.minSpeed + (shipUpgrade.powerLevel * (ship.maxSpeed - ship.minSpeed) / ShipUpgradeData.maxUpdatePowerLevel));
        powerDamage.text = damage.ToString();
        powerSpeed.text = speed.ToString();

        for (int i = 0; i < heroRank.Length; i++)
            heroRank[i].enabled = (i < shipUpgrade.rankLevel);
         

        if (!PlayerData.Instance.shipData[id].unlocked)
        {
            btnUnlock.SetActive(true);
            btnUpgrade.SetActive(false);
            btnUpgradeRank.SetActive(false);
            unlockPrice = ship.crystal;
            unlockCost.text = unlockPrice.ToString();
        }
        else
        {
            btnUnlock.SetActive(false);
            btnUpgrade.SetActive(true);
            btnUpgradeRank.SetActive(true);
            if (shipUpgrade.powerLevel >= ShipUpgradeData.maxUpdatePowerLevel)
            {
                btnUpgrade.GetComponent<Button>().interactable = false;
                upgradeCost.text = "MAX UPGRADE";
            }
            else
            {
                upgradePrice = 500 + PlayerData.Instance.shipData[id].powerLevel * 500;
                upgradeCost.text = upgradePrice.ToString();
            }
            
            if(shipUpgrade.rankLevel >= ShipUpgradeData.maxUpdateRankLevel)
            {
                btnUpgradeRank.GetComponent<Button>().interactable = false;
                rankCost.text = "MAX";
                imgCrystal.enabled = false;
            }            
            else
            {
                btnUpgradeRank.GetComponent<Button>().interactable = true;
                imgCrystal.enabled = true;
                switch (shipUpgrade.powerLevel)
                {
                    case 0:
                        rankPrice = 100;
                        break;
                    case 1:
                        rankPrice = 200;
                        break;
                    case 2:
                        rankPrice = 500;
                        break;
                    case 3:
                        rankPrice = 1000;
                        break;
                    case 4:
                        rankPrice = 1500;
                        break;
                }
                rankCost.text = rankPrice.ToString();
            }
        }
    }
    #endregion;

}
