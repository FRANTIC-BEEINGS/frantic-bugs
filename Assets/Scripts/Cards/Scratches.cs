using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scratches : MonoBehaviour {
    MeshFilter cubeMesh;
    Mesh mesh;
    float TextureResolution = 100f;
    float CardWidth = 2f;
    float CardHeight = 3f;
    float CardDepth = 0.05f;
    float scale = 10;

    void Start() {
        cubeMesh = GetComponent<MeshFilter>();
        mesh = cubeMesh.mesh;
        Vector2[] uvMap = mesh.uv;

        //front
        uvMap[1]  = new Vector2(CardDepth + CardWidth + CardDepth + CardWidth, CardDepth);
        uvMap[0]  = new Vector2(CardDepth + CardWidth + CardDepth, CardDepth);
        uvMap[3]  = new Vector2(CardDepth + CardWidth + CardDepth + CardWidth, CardDepth + CardHeight);
        uvMap[2]  = new Vector2(CardDepth + CardWidth + CardDepth, CardDepth + CardHeight);

        //top
        uvMap[9]  = new Vector2(CardDepth, CardDepth + CardHeight + CardDepth);
        uvMap[8]  = new Vector2(CardDepth + CardWidth, CardDepth + CardHeight + CardDepth);
        uvMap[5]  = new Vector2(CardDepth, CardDepth + CardHeight);
        uvMap[4]  = new Vector2(CardDepth + CardWidth, CardDepth + CardHeight);

        //back
        uvMap[7]  = new Vector2(CardDepth, CardDepth);
        uvMap[6]  = new Vector2(CardDepth + CardWidth, CardDepth);
        uvMap[11] = new Vector2(CardDepth, CardDepth + CardHeight);
        uvMap[10] = new Vector2(CardDepth + CardWidth, CardDepth + CardHeight);

        //bottom
        uvMap[14] = new Vector2(CardDepth, 0f);
        uvMap[15] = new Vector2(CardDepth, CardDepth);
        uvMap[12] = new Vector2(CardDepth + CardWidth, CardDepth);
        uvMap[13] = new Vector2(CardDepth + CardWidth, 0f);

        //left
        uvMap[16] = new Vector2(0f, CardDepth);
        uvMap[17] = new Vector2(0f, CardDepth + CardHeight);
        uvMap[18] = new Vector2(CardDepth, CardDepth + CardHeight);
        uvMap[19] = new Vector2(CardDepth, CardDepth);

        //right
        uvMap[20] = new Vector2(CardDepth + CardWidth, CardDepth);
        uvMap[21] = new Vector2(CardDepth + CardWidth, CardDepth + CardHeight);
        uvMap[22] = new Vector2(CardDepth + CardWidth + CardDepth, CardDepth + CardHeight);
        uvMap[23] = new Vector2(CardDepth + CardWidth + CardDepth, CardDepth);

        float dx = Random.Range(0.0f, (scale - 1)/scale);
        float dy = Random.Range(0.0f, (scale - 1)/scale);
        for (int i = 0; i < 24; ++i) {
            uvMap[i][0] /= (CardDepth + CardWidth + CardDepth + CardWidth);
            uvMap[i][0] /= scale;
            uvMap[i][0] += dx;
            uvMap[i][1] /= (CardDepth + CardHeight + CardDepth);
            uvMap[i][1] /= scale;
            uvMap[i][1] += dy;
        }

        mesh.uv = uvMap;

    }
}
