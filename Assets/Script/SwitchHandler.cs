using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
namespace Lab3{
    public class SwitchHandler : MonoBehaviour{
    public int switchState = 0;
    public GameObject switchBtn;
    public RawImage BG;  
    public void OnChangeSwitch(){        
        switchBtn.transform.DOLocalMoveX(-switchBtn.transform.localPosition.x,0.2f);
        switchState = Math.Sign(-switchBtn.transform.localPosition.x) == 1?1:0;
        updateColor();
    }
    public void updateColor(){
        if (switchState== 1) {
        BG.color=new Color(0f, 255f, 0f);           
        }
        else{
        BG.color=new Color(255f, 0f, 0f);
        }
    }
    public int switchedState(){
        if (switchState==0){
            return 1;
        }
            
        else{
            return 0;
        }
        
    }
    //viet them ham updateswitch stage
}

  
  

}
