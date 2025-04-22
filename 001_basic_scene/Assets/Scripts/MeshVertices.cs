using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class MeshVertices : MonoBehaviour {
    // Start is called before the first frame update
    public Camera cam;
    Color[] colorArray = new Color[4]{Color.red, Color.cyan, Color.blue, Color.black};
    void Update() {
        var mf = GetComponent<MeshFilter>();
        Matrix4x4 localToWorld = transform.localToWorldMatrix;


        for(int i = 0; i < mf.sharedMesh.vertices.Length; ++i) {
            Vector3 world_v = localToWorld.MultiplyPoint3x4(mf.sharedMesh.vertices[i]);
            // var transformed = viewProjMatrixPrev.MultiplyPoint3x4(world_v);
            // //transformed = MatrixProduct(viewProjMatrixPrev, world_v);
            // var coord = (transformed + Vector3.one) * 0.5f;
            // // var coord = new Vector3(transformed.x+1,1-transformed.y,transformed.z+1)*0.5f;

            DrawLine(world_v, world_v+Vector3.up*0.1f, 50, i);

            var coord = GetCoords(world_v);

            Debug.Log($"[{i}] world:{world_v.ToString("F3")} --> coord:{coord.ToString("F3")} || {StrHit(coord, world_v.z, i)}");
        }

    }

    //====> OK
    Vector3 GetCoords(Vector3 worldPos){

        // // Matrix4x4 projMatrix = cam.projectionMatrix;
        // // Matrix4x4 viewMatrix = cam.worldToCameraMatrix;
        // // Matrix4x4 viewProjMatrix = cam.previousViewProjectionMatrix;
        // Matrix4x4 viewProjMatrix = (GL.GetGPUProjectionMatrix(cam.projectionMatrix, true) /* * cam.worldToCameraMatrix*/);
        // // Debug.Log("projMatrix: \n"+projMatrix);
        // // Debug.Log("viewMatrix: \n"+viewMatrix);
        // Debug.Log("viewProjMatrix: \n"+viewProjMatrix);

        // // var transformed = viewProjMatrix.MultiplyPoint3x4(worldPos);
        // Vector4 newWorldPos = new Vector4(worldPos.x, worldPos.y, worldPos.z, 1);
        // // var transformed = MatrixProduct(viewProjMatrix, worldPos);
        // var transformed = viewProjMatrix.MultiplyPoint(newWorldPos);
        // var coord = (transformed + Vector3.one) * 0.5f;


        //https://forum.unity.com/threads/camera-worldtoviewportpoint-math.644383/#post-4321723
        Vector4 worldPos4 = new Vector4(worldPos.x, worldPos.y, worldPos.z, 1.0f);
        // Vector3 worldPos3 = worldPos4;

        var m = cam.projectionMatrix * cam.worldToCameraMatrix;
        var projViewMat = m ;
        Vector4 projPos = projViewMat * worldPos4;
        projPos /=projPos.w;
        // Vector3 viewportPos = new Vector3(projPos.x * 0.5f + 0.5f, projPos.y * 0.5f + 0.5f, -projPos.z);
        Vector3 viewportPos = (projPos + Vector4.one) / 2.0f;


        // Matrix4x4 viewMat = cam.worldToCameraMatrix;
        // Matrix4x4 projMat = cam.projectionMatrix;
        // viewMat[0,3] *= -1;
        // viewMat[1,3] *= -1;

        // Debug.LogWarning($"MATRICES:\n\n\tVIEW:\n{viewMat}\n\n\tPROJECTION:\n{projMat}");
        
        // Vector4 viewPos = viewMat.MultiplyPoint(worldPos4);
        // Vector4 projPos = projMat.MultiplyPoint(viewPos);
        // // Vector3 ndcPos = new Vector3(projPos.x / projPos.w, projPos.y / projPos.w, projPos.z / projPos.w);
        // Vector3 ndcPos = projPos/projPos.w;
        // // Vector3 ndcPos = projPos;
        // Debug.LogWarning($"WORLD: {worldPos}\nVIEW: {viewPos}\nPROJ: {projPos}\nNDCPOS: {ndcPos}");
        // Vector3 viewportPos = new Vector3(ndcPos.x * 0.5f + 0.5f, ndcPos.y * 0.5f + 0.5f, -viewPos.z);

        return viewportPos;
        // return cam.WorldToViewportPoint(worldPos);
    }

    Vector3 MatrixProduct(Matrix4x4 m, Vector3 v) {
        return MatrixProduct(m,new Vector4(v.x,v.y,v.z,1));
    }

    Vector4 MatrixProduct(Matrix4x4 m, Vector4 v) {
        // var x = (m[0,0]*v.x + m[0,1]*v.y + m[0,2]*v.z + m[0,3]);
        // var y = (m[1,0]*v.x + m[1,1]*v.y + m[1,2]*v.z + m[1,3]);
        // var z = (m[2,0]*v.x + m[2,1]*v.y + m[2,2]*v.z + m[2,3]);
        // var w = (m[3,0]*v.x + m[3,1]*v.y + m[3,2]*v.z + m[3,3]);

        var x = (m[0, 0] * v.x + m[0, 1] * v.x + m[0, 2] * v.x + m[0, 3] * v.x);
        var y = (m[1, 0] * v.y + m[1, 1] * v.y + m[1, 2] * v.y + m[1, 3] * v.y);
        var z = (m[2, 0] * v.z + m[2, 1] * v.z + m[2, 2] * v.z + m[2, 3] * v.z);
        var w = (m[3, 0] * v.w + m[3, 1] * v.w + m[3, 2] * v.w + m[3, 3] * v.w);


        return new Vector4(x, y, z, w) / w;
    }

    string StrHit(Vector3 xy, float z, int index) {
        // Vector2 launchIndex = new Vector2(Mathf.FloorToInt(x), Mathf.FloorToInt(y)) + Vector2.one * 0.5f;
        // Vector2 launchDim = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        // Vector2 frameCoord = new Vector2(launchIndex.x, launchDim.y - launchIndex.y - 1) + new Vector2(0.5f, 0.5f);
        // // Vector2 frameCoord = new  Vector2(launchIndex.x, launchIndex.y - 1) + new Vector2(0.5f, 0.5f);

        // Vector2 ndcCoords = frameCoord / new Vector2(launchDim.x - 1, launchDim.y - 1);

        // var g_Zoom = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView * 0.5f);

        // ndcCoords = ndcCoords * 2 - new Vector2(1, 1);
        // ndcCoords = ndcCoords * g_Zoom;

        // float aspectRatio = (float) launchDim.x / (float) launchDim.y;

        // Vector3 viewDirection = Vector3.Normalize(new Vector3(ndcCoords.x * aspectRatio, ndcCoords.y, 1));
        Vector3 viewDirection = GenerateCameraRay3(new Vector2(xy.x, xy.y));
        // Vector3 viewDirection = cam.ViewportPointToRay(xy).direction;
        float t = (z - cam.transform.position.z) / viewDirection.z;

        Vector3 src = cam.transform.position;
        Vector3 dst = (cam.transform.position+viewDirection*t);
        // Vector3 src = cam.transform.position;
        // Vector3 dst = (cam.transform.position+viewDirection*t);
        DrawLine(src,dst, 5, index);
        return $"VIEW DIRECTION: {viewDirection.ToString("F3")}  <->  world:{(cam.transform.position + viewDirection * t).ToString("F3")}";
    }

    void DrawLine(Vector3 src, Vector3 dst, int thickness, int index){
        for(float delta_y = -0.01f*thickness; delta_y <= 0.01f*thickness; delta_y += 0.01f){
            for(float delta_x = -0.05f; delta_x <= 0.05f; delta_x += 0.01f){
                // Debug.DrawLine(cam.transform.position, cam.transform.position+viewDirection*t, colorArray[index], 1);
                Debug.DrawLine(src+Vector3.up*delta_y+Vector3.right*delta_x, dst+Vector3.up*delta_y+Vector3.right*delta_x, colorArray[index]);
            }
        }
    }

    //====> OK
    Vector3 GenerateCameraRay3(Vector2 launchIndex){

        //transformar [0,1] en [-1,1]
        // launchIndex *= 2;
        // launchIndex += Vector2.one*.25f;

        Matrix4x4 m_View2NDC = Matrix4x4.Translate(-Vector3.one) * Matrix4x4.Scale(Vector3.one * 2);
        var m = cam.projectionMatrix * cam.worldToCameraMatrix;
        Matrix4x4 mInv = m.inverse * m_View2NDC;

        var ap = new Vector3(launchIndex.x, launchIndex.y, 0);
        var p0 = mInv.MultiplyPoint(ap);
        ap.z = 1;
        var p1 = mInv.MultiplyPoint(ap);
        return (p1-p0).normalized;
    }
    Vector3 GenerateCameraRay2(Vector2 launchIndex){
        // uint2 launchIndex = DispatchRaysIndex().xy+0.5f;

        launchIndex.x *= cam.pixelWidth;
        launchIndex.y *= cam.pixelHeight;

        launchIndex += Vector2.one*0.5f;

        Vector2Int launchDim = new Vector2Int(cam.pixelWidth, cam.pixelHeight);

        Vector2 frameCoord = new Vector2(launchIndex.x, launchDim.y - launchIndex.y - 1) + Vector2.one*0.5f;

        Vector2 ndcCoords = frameCoord / new Vector2(launchDim.x - 1, launchDim.y - 1);


        ndcCoords = ndcCoords * 2 - Vector2.one;

        var g_Zoom = Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView * 0.5f);

        ndcCoords = ndcCoords * g_Zoom;

        float aspectRatio = (float)launchDim.x / (float)launchDim.y;

        //cambié el signo de 'ndcCoords.x'
        Vector3 viewDirection = (new Vector3(-ndcCoords.x * aspectRatio, ndcCoords.y, 1)).normalized;

        // Rotate the ray from view space to world space.
        return  MatrixProduct(cam.cameraToWorldMatrix, viewDirection);
    }
    Vector3 GenerateCameraRay(Vector2 xyPixels) {
        Debug.LogError(xyPixels);
        xyPixels.x = Mathf.RoundToInt(xyPixels.x)*cam.pixelWidth;
        xyPixels.y = Mathf.RoundToInt(xyPixels.y)*cam.pixelHeight;
        
        // center in the middle of the pixel.
        xyPixels += Vector2.one * 0.5f;

        Vector2 launchDim = new Vector2(cam.pixelWidth, cam.pixelHeight);
        Vector2 screenPos = xyPixels / launchDim * 2.0f - Vector2.one;

        // Un project the pixel coordinate into a ray.
        Matrix4x4 _InvCameraViewProj = (GL.GetGPUProjectionMatrix(cam.projectionMatrix, false) * cam.worldToCameraMatrix).inverse;
        Vector3 world = MatrixProduct(_InvCameraViewProj, new Vector3(screenPos.x, screenPos.y, 1));


        var origin = cam.transform.position;
        var direction = (world - origin).normalized;

        Debug.LogError($"origin: {origin}\nworld: {world}");
        Debug.DrawLine(origin, world, Color.magenta);
        // Debug.DrawLine(origin, origin+direction*10, Color.blue, 10);
        // Debug.DrawLine(new Vector3(32.535f, -16.814f, 50.000f), 
        // new Vector3(-32.135f, -16.814f, 50.000f), Color.black, 100);
        return direction;
    }
}

