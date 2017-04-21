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

        private string scaleX;
        private float valueScaleX = 0;

        private string scaleY;
        private float valueScaleY = 0;

        private string scaleZ;
        private float valueScaleZ = 0;

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
            scaleX = valueScaleX.ToString();
            scaleY = valueScaleY.ToString();
            scaleZ = valueScaleZ.ToString();
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
            if (blockID == "") valueID = 0; 
            else valueID = int.Parse(blockID);

            if (posX == "") valuePosX = 0;
            else valuePosX = float.Parse(posX);

            if (posY == "") valuePosY = 0;
            else valuePosY = float.Parse(posY);

            if (posZ == "") valuePosZ = 0;
            else valuePosZ = float.Parse(posZ);

            if (rotX == "") valueRotX = 0;
            else valueRotX = float.Parse(rotX);

            if (rotX == "") valueRotY = 0;
            else valueRotY = float.Parse(rotY);

            if (rotX == "") valueRotZ = 0;
            else valueRotZ = float.Parse(rotZ);

            if (scaleX == "") valueScaleX = 0;
            else valueScaleX = float.Parse(scaleX);

            if (scaleY == "") valueScaleY = 0;
            else valueScaleY = float.Parse(scaleY);

            if (scaleZ == "") valueScaleZ = 0;
            else valueScaleZ = float.Parse(scaleZ);

            GameObject Nlock;
            if (blockToSpawn == null)
            {
                SpawnChild();
            }
            
            Vector3 relativePosition = new Vector3(Machine.Active().GetBlock(410).transform.position.x + valuePosX, Machine.Active().GetBlock(410).transform.position.y + valuePosY, Machine.Active().GetBlock(410).transform.position.z + valuePosZ);
            Quaternion relativeRotation = new Quaternion(valueRotX, valueRotY, valueRotZ, Machine.Active().GetBlock(410).transform.rotation.w);
            //Vector3 scale = new Vector3(valueScaleX, valueScaleY, valueScaleZ);
            Nlock = (GameObject)GameObject.Instantiate(blockToSpawn.gameObject,relativePosition, relativeRotation);   
            Nlock.SetActive(true);
            XDataHolder xDataHolder = new XDataHolder { WasSimulationStarted = true };
            blockToSpawn.OnSave(xDataHolder);
            Nlock.GetComponent<BlockBehaviour>().OnLoad(xDataHolder);
            Nlock.GetComponent<Rigidbody>().isKinematic = false;
            Nlock.transform.localScale *= 1f;
            Nlock.transform.SetParent(Machine.Active().SimulationMachine);
           /* var st = Nlock.transform;
            var at = Machine.Active().GetBlock(410).transform;
            st.position = at.TransformDirection(new Vector3(valuePosX,valuePosY,valuePosZ)) + at.position;
            st.rotation = at.rotation * Quaternion.Euler(valueScaleX, valueScaleY, valueScaleZ);*/
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

            GUILayout.Label("Block Rotation (relative to the Automatron)");

            GUILayout.BeginHorizontal();
            GUILayout.Label("   X");
            GUILayout.Label("   Y");
            GUILayout.Label("   Z");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            rotX = GUILayout.TextField(rotX);
            rotY = GUILayout.TextField(rotY);
            rotZ = GUILayout.TextField(rotZ);
            GUILayout.EndHorizontal();

            GUILayout.Label("Block Scale");

            GUILayout.BeginHorizontal();
            GUILayout.Label("   X");
            GUILayout.Label("   Y");
            GUILayout.Label("   Z");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            scaleX = GUILayout.TextField(scaleX);
            scaleY = GUILayout.TextField(scaleY);
            scaleZ = GUILayout.TextField(scaleZ);
            GUILayout.EndHorizontal();

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
                       ",posZ:"+posZ+
                       ",rotX:"+rotX+
                       ",rotY:"+rotY+
                       ",rotZ:"+rotZ+
                       ",scaleX:"+scaleX+
                       ",scaleY:"+scaleY+
                       ",scaleZ:"+scaleZ+"}";
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
                    case "rotX":
                        rotX = val;
                        break;
                    case "rotY":
                        rotY = val;
                        break;
                    case "rotZ":
                        rotZ = val;
                        break;
                    case "scaleX":
                        scaleX = val;
                        break;
                    case "scaleY":
                        scaleY = val;
                        break;
                    case "scaleZ":
                        scaleZ = val;
                        break;
                }
            }  
            UpdateTitle();
        }
    }
}