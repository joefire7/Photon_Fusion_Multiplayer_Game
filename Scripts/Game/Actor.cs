using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    // Define properties that can be override by specific player class
    protected string playerName { get; set; }
    protected int playerHealth { get; set; }
    protected int playerAttackDamage { get; set; }
    protected int playerMana { get; set; }
    protected float playerSpeed { get; set; }
    protected int playerDefend { get; set; }
    
   
    // Method to be overridden by child to define player powers
    public virtual void Power1() { }
    public virtual void Power2() { }
    public virtual void Power3() { }
    public virtual void Power4() { }
    public virtual void Power5() { }
    public virtual void Power6() { }


}
