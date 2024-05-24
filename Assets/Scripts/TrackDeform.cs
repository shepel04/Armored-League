using UnityEngine;



    [System.Serializable]
    public class IntArray
    {
        public int[] intArray;
        public IntArray(int[] newIntArray)
        {
            intArray = newIntArray;
        }
    }


    public class TrackDeform : MonoBehaviour
    {
        /*
         * This script is attached to each track in the tank.
         * This script controls the deformation of the track.
        */

        public int anchorNum;
        public Transform[] anchorArray;
        public string[] anchorNames;
        public string[] anchorParentNames;
        public float[] widthArray;
        public float[] heightArray;
        public float[] offsetArray;
        public float[] initialPosArray;
        public Vector3[] initialVertices;
        public IntArray[] movableVerticesList;

        // For editor script.
        public bool hasChanged;


        Mesh thisMesh;
        Vector3[] currentVertices;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        {
            thisMesh = GetComponent<MeshFilter>().mesh;
            thisMesh.MarkDynamic();
            currentVertices = new Vector3[initialVertices.Length];

            // Check the anchor wheels.
            for (int i = 0; i < anchorArray.Length; i++)
            {
                if (anchorArray[i] == null)
                {
                    // Find the anchor wheel with reference to the name.
                    if (string.IsNullOrEmpty(anchorNames[i]) == false && string.IsNullOrEmpty(anchorParentNames[i]) == false)
                    {
                        anchorArray[i] = transform.parent.Find(anchorParentNames[i] + "/" + anchorNames[i]);
                    }
                    else
                    {
                        Debug.LogError("'Anchor wheels' of the Track are not assigned.");
                        Destroy(this);
                        return;
                    }
                }
            }
        }


        void Update()
        {
            Deform_Vertices();
        }


        void Deform_Vertices()
        {
            initialVertices.CopyTo(currentVertices, 0);

            // Move the vertices according to the anchor wheels.
            for (int i = 0; i < anchorArray.Length; i++)
            {
                var tempDist = anchorArray[i].localPosition.x - initialPosArray[i];
                for (int j = 0; j < movableVerticesList[i].intArray.Length; j++)
                {
                    currentVertices[movableVerticesList[i].intArray[j]].y += tempDist;
                }
            }

            thisMesh.vertices = currentVertices;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
        }


        void OnDestroy()
        { // Avoid memory leak.
            if (thisMesh)
            {
                Destroy(thisMesh);
            }
        }

    }

