using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Lab3
{
    public class Manager : MonoBehaviour
    {
        
        
        [SerializeField]
        private CanvasGroup _canvasLayer1;       
        [SerializeField]
        private InputField _broker;
        [SerializeField]
        private InputField _user_name;
        [SerializeField]
        private InputField _password;
        [SerializeField]
        private string broker = "mqttserver.tk";
        private string user_name = "bkiot";
        [SerializeField]
        private string password ="12345678";   
        [SerializeField]     
        private Text msg_layer1;
        
        
     



        /// <summary>
        /// Layer 2 elements
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasLayer2;
        [SerializeField]
        private Text temperature;
        [SerializeField]
        private Text humidity;
 
        
        [SerializeField]
        private SwitchHandler LedSwitch;
        [SerializeField]
        private SwitchHandler PumpSwitch;
        [SerializeField]     
        private Text msg_layer2;
        
 
        //Layer3
        
        
        

        /// <summary>
        [SerializeField]
        private CanvasGroup _canvas_LayerButton;
        /// </summary>
        

        private Tween twenFade;

        private int led_status = 0;
        private int pump_status = 0;
        

        IEnumerator _IEWaitConnect()
        {
            GetComponent<Mqtt>().checkConnect();
            yield return new WaitForSeconds(3);
            Debug.Log(GetComponent<Mqtt>().mqttConnected);
            if (GetComponent<Mqtt>().mqttConnected){                
                SwitchLayer12();
            }

        }
        IEnumerator _IEWaitPublishLed()
        {
            
            string current_msg_led=GetComponent<Mqtt>().msg_received_from_topic_led;

            GetComponent<Mqtt>().Publish_Device("LED",led_status==1?"ON":"OFF");               
            yield return new WaitForSeconds(3.0f);
            if(current_msg_led == GetComponent<Mqtt>().msg_received_from_topic_led)
            {
                
                setMsg("Update Led Failed","Layer2");
                yield return new WaitForSeconds(1);
                setMsg("","Layer2");
                _canvas_LayerButton.interactable=true;
                _canvas_LayerButton.blocksRaycasts = true;
                
            }
            else
            {   
                FadeIn(_canvasLayer2,3.0f);
                setMsg("Update Led Successful","Layer2");
                //StartCoroutine(_IESwitchLed());
                yield return new WaitForSeconds(1);
                setMsg("","Layer2");
                _canvas_LayerButton.interactable=true;
                _canvas_LayerButton.blocksRaycasts = true;
            }

        }
        IEnumerator _IEWaitPublishPump()
        {
            
            string current_msg_pump=GetComponent<Mqtt>().msg_received_from_topic_pump;

            
            GetComponent<Mqtt>().Publish_Device("PUMP",pump_status==1?"ON":"OFF");                 
            yield return new WaitForSeconds(3);
            if(current_msg_pump == GetComponent<Mqtt>().msg_received_from_topic_pump)
            {
                setMsg("Update Pump Failed","Layer2");
                yield return new WaitForSeconds(1);
                setMsg("","Layer2");
                _canvas_LayerButton.interactable=true;
                _canvas_LayerButton.blocksRaycasts = true;
            }
            else
            {   
                setMsg("Update Pump Successful","Layer2");
                yield return new WaitForSeconds(1);
                //StartCoroutine(_IESwitchPump());
                setMsg("","Layer2");
                _canvas_LayerButton.interactable=true;
                _canvas_LayerButton.blocksRaycasts = true;        
            }

        }
        
        public void authenticate()
        {
            string msg="";
            if (_broker.text != broker){
                msg="Invalid Broker URI";
                setMsg(msg,"Layer1");
            }
            else if (_user_name.text != user_name){
                msg="Invalid User name";
                setMsg(msg,"Layer1");               
            }
            else if (_password.text != password){
                msg="Invalid Password";
                setMsg(msg,"Layer1");                
            }
            else{
                setMsg("Connecting...","Layer1");
                StartCoroutine(_IEWaitConnect());                
            }         
            
            //}
        }
        
        public void setMsg(string msg, string layer){
            switch(layer){
                case "Layer1":
                    msg_layer1.text = msg;
                    break;
                case "Layer2":
                    msg_layer2.text = msg;
                    break;                
            }
                
            
        }

        public void Update_Status(data_ss _status_data)
        {
            
            temperature.text = _status_data.temperature + "°C";                        
            humidity.text = _status_data.humidity + "°C";
            // if(_status_data.device_status=="1")
            //     _btn_config.interactable = true;

        }
        //subscribes then update
        public void Update_Device(data_device _device_data)
        {
            switch(_device_data.device){
                case "LED":
                    led_status=_device_data.status == "ON"?1:0;
                    
                    break;
                case "PUMP":
                    pump_status=_device_data.status == "ON"?1:0;
                    break;
            }            
            if (led_status!=LedSwitch.switchState){
                StartCoroutine(_IESwitchLed()); 
            }
            if (pump_status!=PumpSwitch.switchState){
                StartCoroutine(_IESwitchPump()); 
            }
        }
        public void OnSwitchLedClicked(){
            setMsg("Updating Led...","Layer2");                      
            led_status = LedSwitch.switchedState();
            _canvas_LayerButton.interactable=false;
            _canvas_LayerButton.blocksRaycasts = false;
            StartCoroutine(_IEWaitPublishLed());
            
        }
        public void OnSwitchPumpClicked(){
            
            setMsg("Updating Pump...","Layer2");
            pump_status = PumpSwitch.switchedState();
            _canvas_LayerButton.interactable=false;
            _canvas_LayerButton.blocksRaycasts = false;
            StartCoroutine(_IEWaitPublishPump());            
            
        }
        IEnumerator _IESwitchLed()
        {
            LedSwitch.GetComponent<Button>().interactable = false;
            LedSwitch.switchBtn.GetComponent<Button>().interactable=false;
            if(led_status==LedSwitch.switchState){
                yield return new WaitForSeconds(0.15f);
                LedSwitch.GetComponent<Button>().interactable = true;
                LedSwitch.switchBtn.GetComponent<Button>().interactable=true;
            }
            else{
                LedSwitch.OnChangeSwitch();
                yield return new WaitForSeconds(0.3f);
                LedSwitch.GetComponent<Button>().interactable = true;
                LedSwitch.switchBtn.GetComponent<Button>().interactable=true;
            }
        }
        IEnumerator _IESwitchPump()
        {
            PumpSwitch.GetComponent<Button>().interactable = false;
            PumpSwitch.switchBtn.GetComponent<Button>().interactable=true;
            if(pump_status==PumpSwitch.switchState){
                yield return new WaitForSeconds(0.15f);
                PumpSwitch.GetComponent<Button>().interactable = true;
                PumpSwitch.switchBtn.GetComponent<Button>().interactable=true;
            }
            else{
                PumpSwitch.OnChangeSwitch();
                yield return new WaitForSeconds(0.3f);
                PumpSwitch.GetComponent<Button>().interactable = true;
                PumpSwitch.switchBtn.GetComponent<Button>().interactable=true;
            }
        }

        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }



        IEnumerator _IESwitchLayer12()
        {
            
            if (_canvasLayer1.interactable == true)
            {                
                FadeOut(_canvasLayer1, 0.25f);                
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer2, 0.25f);
                setLayer();
            }
            else
            {
                FadeOut(_canvasLayer2, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer1, 0.25f);
                setLayer();
                GetComponent<Mqtt>().Disconnect();
                setMsg("Disconnected.","Layer1");
            }
        }
        

        
        public void SwitchLayer12()
        {
            StartCoroutine(_IESwitchLayer12());
        }
        
        public void setLayer(){
            if(_canvasLayer1.interactable==true){
                FadeOut(_canvasLayer2, 0.25f);
                
            }
            if(_canvasLayer2.interactable==true){
                FadeOut(_canvasLayer1, 0.25f);                
            }
            
        }
        public void onQuitClicked(){
            Application.Quit();
        }
        
    }
}