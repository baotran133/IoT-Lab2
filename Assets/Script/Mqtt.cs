using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace Lab3
{
    

    public class data_ss
    {
        public string temperature { get; set; }        
        public string humidity { get; set; }
    }

    
    public class data_device
    {
        public string device { get; set; }
        public string status { get; set; }

    }
    

    public class Mqtt : M2MqttUnityClient
    {
        public bool auto_Connect = true;
        public List<string> topics = new List<string>();
        

        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_led = "";
        public string msg_received_from_topic_pump = "";


        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public data_ss _status_data;

        [SerializeField]
        public data_device _device_data;
        
        [SerializeField]

        public bool mqttConnected = false;
        
        

        
        
        // public ControlFan_Data _controlFan_data;
        


        // public void PublishConfig()
        // {
        //     _config_data = new Config_Data();
        //     GetComponent<Manager>().Update_Config_Value(_config_data);
        //     string msg_config = JsonConvert.SerializeObject(_config_data);
        //     client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        //     Debug.Log("publish config");
        // }

        // public void PublishFan()
        // {
        //     _controlFan_data = GetComponent<Manager>().Update_ControlFan_Value(_controlFan_data);
        //     string msg_config = JsonConvert.SerializeObject(_controlFan_data);
        //     client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        //     Debug.Log("publish fan");


        // }
        
        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            //SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        

        protected override void SubscribeTopics()
        {

            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }
        public void checkConnect()
        {
            base.Connect();           
            
        }
        public void checkPublish()
        {
            
            SubscribeTopics();
            
        }
        public void Publish_Device(string device,string status)
        {
            Debug.Log(device);
            Debug.Log(status);
            var my_jsondata = new
            {
                device = device,
                status = status

            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(my_jsondata);
            Debug.Log(json_data);
            client.Publish("/bkiot/1710614/"+device.ToLower(), System.Text.Encoding.UTF8.GetBytes(json_data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("Test message published");            
        }
        public void PublishStatus(string temperature,string humidity)
        {
            var my_jsondata = new
            {
                temperature = temperature,
                humidity = humidity

            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(my_jsondata);
            Debug.Log(json_data);
            client.Publish("/bkiot/1710614/status", System.Text.Encoding.UTF8.GetBytes(json_data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("Test message published");            
        }
        protected override void OnConnected()
        {
            //Publish_Device("LED","ON");
            //Publish_Device("PUMP","OFF");
            //PublishStatus(21,39);
            // Debug.LogFormat("Connected to {0}:{1}...\n", brokerAddress, brokerPort.ToString());
            mqttConnected =true;
            SubscribeTopics();
        }
        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
            GetComponent<Manager>().setMsg(errorMessage,"Layer1");           
        }


        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
            
            GetComponent<Manager>().setMsg("Disconnected.","Layer1");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
            GetComponent<Manager>().setMsg("CONNECTION LOST!","Layer1");
        }
        



        protected override void Start()
        {

            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            //StoreMessage(msg);
            if (topic == topics[0]){
                ProcessMessageStatus(msg);
            }  
            if (topic == topics[1]){               
                ProcessMessageDevice(msg);
            }
            if (topic == topics[2]){               
                ProcessMessageDevice(msg);
            }
                
        }

        private void ProcessMessageStatus(string msg)
        {
            _status_data = JsonConvert.DeserializeObject<data_ss>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<Manager>().Update_Status(_status_data);

        }
        private void ProcessMessageDevice(string msg)
        {
            _device_data = JsonConvert.DeserializeObject<data_device>(msg);
            switch(_device_data.device){
                case "LED":
                    msg_received_from_topic_led = msg;
                    break;
                case "PUMP":
                    msg_received_from_topic_pump = msg;
                    break;
            }
            GetComponent<Manager>().Update_Device(_device_data);

        }

        // private void ProcessMessageControl(string msg)
        // {
        //     _controlFan_data = JsonConvert.DeserializeObject<ControlFan_Data>(msg);
        //     msg_received_from_topic_control = msg;
        //     GetComponent<Manager>().Update_Control(_controlFan_data);

        // }
        
        private void OnDestroy()
        {
            Disconnect();
        }

        

        public void UpdateConfig()
        {
           
        }

        public void UpdateControl()
        {

        }
    }
}