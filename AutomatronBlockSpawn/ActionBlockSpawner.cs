﻿using System;
using System.Collections;
using System.Collections.Generic;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;
using spaar.Mods.Automatron;
using spaar.Mods.Automatron.Actions;

namespace Sylver.AutomatronExtention
{
    public class ActionBlockSpawner : BlockSpecificAction
    {
        public override string Title { get; set; } = "Spawn Block";

        private string blockID;
        private int valueID = 0;

        private string posX;
        private int valueX = 0;

        private string posY;
        private int valueY = 0;

        private string posZ;
        private int valueZ = 0;

        private List<int> MatchingIDs = new List<int>();

        public BlockBehaviour blockToSpawn;

        public override void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
        {
            base.Create(cb, hideCb);

            blockID = valueID.ToString();
            posX = valueX.ToString();
            posY = valueY.ToString();
            posZ = valueZ.ToString();


            List<string> list = new List<string>();

            foreach (BlockPrefab pair in PrefabMaster.BlockPrefabs.Values)
            {
                list.Add(pair.gameObject.GetComponent<MyBlockInfo>().blockName);
                MatchingIDs.Add(pair.ID);
            }
        }

        private void UpdateTitle()
        {
            if (Game.IsSimulating) return;
        }

        public override void Trigger()
        {
            GameObject Nlock;
            if (blockToSpawn == null)
            {
                SpawnChild();
            }

            Nlock = (GameObject)GameObject.Instantiate(blockToSpawn.gameObject,Machine.Active().transform.position, Machine.Active().transform.rotation);
            Nlock.SetActive(true);
            XDataHolder xDataHolder = new XDataHolder { WasSimulationStarted = true };
            blockToSpawn.OnSave(xDataHolder);
            Nlock.GetComponent<BlockBehaviour>().OnLoad(xDataHolder);
            Nlock.GetComponent<Rigidbody>().isKinematic = false;
            Nlock.transform.SetParent(Machine.Active().SimulationMachine);
        }

        public void IDChanged(int ID)
        {
            if (BlockMapper.CurrentInstance == null)
            {
                SpawnChild();
            }
        }

        protected virtual IEnumerator UpdateMapper()
        {
            if (BlockMapper.CurrentInstance == null)
            {
                yield break;
            }
            while (Input.GetMouseButton(0))
            {
                yield return null;
            }

            BlockMapper.CurrentInstance.Copy();
            BlockMapper.CurrentInstance.Paste();
            yield break;
        }

        private void SpawnChild()
        {
            if (blockToSpawn != null)
            {
                GameObject.Destroy(blockToSpawn.gameObject);
            }
                blockToSpawn = GameObject.Instantiate(PrefabMaster.BlockPrefabs[MatchingIDs[valueID]].blockBehaviour);
                blockToSpawn.gameObject.SetActive(false);
                //blockToSpawn.transform.SetParent(this.transform);  
        }


        protected override void DoWindow(int id)
        {

            GUILayout.Label("Block ID");
            blockID = GUILayout.TextField(blockID);

            GUILayout.Label("Block Position (relative to the Automatron)");

            GUILayout.BeginHorizontal();
            GUILayout.Label("X");
            GUILayout.Label("Y");
            GUILayout.Label("Z");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            posX = GUILayout.TextField(posX);
            posY = GUILayout.TextField(posY);
            posZ = GUILayout.TextField(posZ);
            GUILayout.EndHorizontal();



            GUILayout.FlexibleSpace();     

            if (GUILayout.Button("Save"))
            {
                Close();
            }

            GUI.DragWindow();
        }

        protected override void Close()
        {
            configuring = false;
            currentCallback();
        }

        public override string Serialize()
        {
            var data = "Change Spawned Block ID and position?" +
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