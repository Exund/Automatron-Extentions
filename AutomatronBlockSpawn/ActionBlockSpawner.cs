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

		private string[] sValues = new String[9];
		private Vector3[] fValues = new Vector3[] { Vector3.zero,Vector3.zero,Vector3.one };
		private string[] names = new string[]{"posX","posY","posZ",
			"rotX","rotY","rotZ","scaleX","scaleX","scaleY","scaleZ"};
		private enum value { pos=0, rot=1, scale=2 };
		private enum dim { x=0, y=1, z=2};
		
        public BlockBehaviour blockToSpawn;

        public override void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
        {
            base.Create(cb, hideCb);
            UpdateTitle();
            blockID = valueID.ToString();
			sValues = new string[9];
			for(int i = 0; i<9; i++) {
				sValues[i] = fValues[i/3][i%3].ToString();
			}
        }

        private void UpdateTitle()
        {
            if (Game.IsSimulating) return;

            if (blockID != "")
            {
                this.Title = "Spawn " + PrefabMaster.BlockPrefabs[valueID].blockBehaviour.gameObject.GetComponent<MyBlockInfo>().blockName;
            }
        }

        public override void Trigger(AutomatronBlock automatron)
        {
			try{ valueID = int.Parse(blockID);
			} catch { valueID = 0; }
			for(int i = 0;i<9;i++)
			{
				Debug.Log("S:"+sValues[i]);
				try
				{ fValues[i/3][i%3] = float.Parse(sValues[i]);
				} catch{ fValues[i/3][i%3] = 0; }
			}
			Debug.Log(fValues[0]);
			Debug.Log(fValues[1]);
			Debug.Log(fValues[2]);

			if(valueID == 0) fValues[(int)value.pos][(int)dim.z] += 0.5f;

            GameObject Nlock;
            if (blockToSpawn == null)
            {
                SpawnChild();
            }

            Nlock = (GameObject)GameObject.Instantiate(blockToSpawn.gameObject);

            var st = Nlock.transform;
            var at = automatron.transform;
            st.parent = at;
            /*st.position = at.TransformDirection(new Vector3(valuePosX, valuePosY, valuePosZ)) + at.position;
            st.rotation = at.rotation * Quaternion.Euler(valueRotX, valueRotY, valueRotZ);*/
            st.localPosition = fValues[(int)value.pos];
            st.localRotation = Quaternion.Euler(fValues[(int)value.rot]);
              
            Nlock.SetActive(true);
            XDataHolder xDataHolder = new XDataHolder { WasSimulationStarted = true };
            blockToSpawn.OnSave(xDataHolder);
            Nlock.GetComponent<BlockBehaviour>().OnLoad(xDataHolder);
            Nlock.GetComponent<Rigidbody>().isKinematic = false;
            Nlock.transform.SetParent(Machine.Active().SimulationMachine);
			st.localScale = fValues[(int)value.scale];

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

			for(int i = 0; i<9; i++)
			{
				if(i%3==0)
				{
					switch(i)
					{
						case 0:
							GUILayout.Label("Block Position (relative to the Automatron)");
							break;
						case 3:
							GUILayout.EndHorizontal();
							GUILayout.Label("Block Rotation (relative to the Automatron)");
							break;
						case 6:
							GUILayout.EndHorizontal();
							GUILayout.Label("Block Scale");
							break;
					}
					GUILayout.BeginHorizontal();
					GUILayout.Label("   X");
					GUILayout.Label("   Y");
					GUILayout.Label("   Z");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}
				sValues[i] = GUILayout.TextField(sValues[i]);
			}
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
                       "{blockID:"+blockID;
			for(int i = 0; i<9; i++)
			{
				data += ","+names[i]+":"+sValues[i];
			}
            return data;
        }

        public override void Load(string data)
        {
            base.Load(data);

			sValues = new string[9];
			for(int i = 0; i<9; i++) sValues[i] = i<6?"0":"1";

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
                    default:
						var index = Array.IndexOf(names,key);
						if(index!=-1) {
							sValues[index] = val;
						}
                        break;
                }
            }  
            UpdateTitle();
        }
	}
}