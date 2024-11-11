using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Troop
{
    public int soldiers = 0;
    public List<Pop> pops = new List<Pop>();

    public void takeCasualties(int amount){
        if (amount > soldiers){
            amount = soldiers;
        }
        foreach (Pop pop in pops){
            int casualtiesForPop = amount;
            if (pop.population - amount < 0){
                casualtiesForPop = amount - Mathf.Abs(pop.population - amount);
            }
            pop.changePopulation(casualtiesForPop);
        }
    }
}
