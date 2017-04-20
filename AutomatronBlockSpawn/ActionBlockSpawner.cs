using System;
using System.Collections;
using System.Collections.Generic;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;
using spaar.Mods.Automatron;
using spaar.Mods.Automatron.Actions;

namespace Sylver.AutomatronExtention
{
    public class ActionBlockSpawner : spaar.Mods.Automatron.Action
    {
        public override string Title { get; set; } = "Spawn Block";

        private string blockID;
        private int valueID = 0;

        private string posX;
        private float valuePosX = 0;

        private string posY;
        private float valuePosY = 0;

        private string posZ;
        private float valuePosZ = 0;

        private string rotX;
        private float valueRotX = 0;

        private string rotY;
        private float valueRotY = 0;

        private string rotZ;
        private float valueRotZ = 0;

        public BlockBehaviour blockToSpawn;

        public override void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
        {
            base.Create(cb, hideCb);
            UpdateTitle();
            blockID = valueID.ToString();
            posX = valuePosX.ToString();
            posY = valuePosY.ToString();
            posZ = valuePosZ.ToString();
            rotX = valueRotX.ToString();
            rotY = valueRotY.ToString();
            rotZ = valueRotZ.ToString();
        }

        private void UpdateTitle()
        {
            if (Game.IsSimulating) return;

            if (blockID != "")
            {
                Title = "Spawn " + PrefabMaster.BlockPrefabs[valueID].blockBehaviour.gameObject.GetComponent<MyBlockInfo>().blockName + "\nat " +posX+" "+posY+" "+posZ;
            }
        }

        public override void Trigger()
        {
            valueID = int.Parse(blockID);
            valuePosX = float.Parse(posX);
            valuePosY = float.Parse(posY);
            valuePosZ = float.Parse(posZ);
            valueRotX = float.Parse(rotX);
            valueRotY = float.Parse(rotY);
            valueRotZ = float.Parse(rotZ);

            GameObject Nlock;
            if (blockToSpawn == null)
            {
                SpawnChild();
            }
            
            Vector3 relativePosition = new Vector3(Machine.Active().GetBlock(410).transform.position.x + valuePosX, Machine.Active().GetBlock(410).transform.position.y + valuePosY, Machine.Active().GetBlock(410).transform.position.z + valuePosZ);
            Quaternion relativeRotation = new Quaternion(valueRotX * 1f, valueRotY * 1f, valueRotZ * 1f,1f);
            Nlock = (GameObject)GameObject.Instantiate(blockToSpawn.gameObject,relativePosition, Machine.Active().GetBlock(410).transform.rotation);
            Nlock.SetActive(true);
            XDataHolder xDataHolder = new XDataHolder { WasSimulationStarted = true };
            blockToSpawn.OnSave(xDataHolder);
            Nlock.GetComponent<BlockBehaviour>().OnLoad(xDataHolder);
            Nlock.GetComponent<Rigidbody>().isKinematic = false;
            Nlock.transform.localScale *= 1;
            Nlock.transform.SetParent(Machine.Active().SimulationMachine);       
        }

        public void IDChanged(int ID)
        {
            if (BlockMapper.CurrentInstance == null)
            {
                SpawnChild();
            }
        }

        private void SpawnChild()
        {
            if (blockToSpawn != null)
            {
                GameObject.Destroy(blockToSpawn.gameObject);
            }
                blockToSpawn = GameObject.Instantiate(PrefabMaster.BlockPrefabs[valueID].blockBehaviour);
                blockToSpawn.gameObject.SetActive(false); 
        }


        protected override void DoWindow(int id)
        {

            GUILayout.Label("Block ID");
            blockID = GUILayout.TextField(blockID);

            GUILayout.Label("Block Position (relative to the Automatron)");

            GUILayout.BeginHorizontal();
            GUILayout.Label("   X");
            GUILayout.Label("   Y");
            GUILayout.Label("   Z");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            posX = GUILayout.TextField(posX);
            posY = GUILayout.TextField(posY);
            posZ = GUILayout.TextField(posZ);
            GUILayout.EndHorizontal();

            /*GUILayout.Label("Block Rotation (relative to the Automatron)");

            GUILayout.BeginHorizontal();
            GUILayout.Label("   X");
            GUILayout.Label("   Y");
            GUILayout.Label("   Z");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            rotX = GUILayout.TextField(rotX);
            rotY = GUILayout.TextField(rotY);
            rotZ = GUILayout.TextField(rotZ);
            GUILayout.EndHorizontal();*/

            GUILayout.FlexibleSpace();     

            if (GUILayout.Button("Save"))
            {
                Close();
            }

            GUI.DragWindow();
            UpdateTitle();
        }

        protected override void Close()
        {
            configuring = false;
            currentCallback();
        }

        public override string Serialize()
        {
            var data = "Spawn Block?" +
                       "{blockID:"+blockID +
                       ",posX:"+posX+
                       ",posY:"+posY+
                       ",posZ:"+posZ+"}";
            return data;
        }

        public override void Load(string data)
        {
            base.Load(data);

            data = data.Replace("{", "").Replace("}", "");
            var pairs = data.Split(',');
            foreach (var pair in pairs)
            {
                var key = pair.Split(':')[0];
                var val = pair.Split(':')[1];

                switch (key)
                {
                    case "blockID":
                        blockID = val;
                        break;
                    case "posX":
                        posX = val;
                        break;
                    case "posY":
                        posY = val;
                        break;
                    case "posZ":
                        posZ = val;
                        break;

                }
            }  
            UpdateTitle();
        }
    }
}