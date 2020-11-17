using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//public void Example()
//{
//    User user = new User();
//    StartCoroutine(Post(Api.login, JsonUtility.ToJson(user),Example,Example));
//}
public static class Server
{
    private static string IP = "0.0.0.0:0";
    public enum Api { none, login, walletSaving }
    private static string[] stringApi = new string[] { "", "/employe/login/", "/wallet/save" };


    public static IEnumerator Post(Api api, string json, Action<UnityWebRequest> Accept, Action Decline, bool debug = false)
    {
        if (debug)
        {
            Debug.Log(json);
        }
        //byte[] dataToPut = System.Text.Encoding.UTF8.GetBytes(j);
        UnityWebRequest www = UnityWebRequest.Put("http://" + IP + stringApi[(int)api], json);

        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        Reaction(www, Accept, Decline, debug);
    }

    public static IEnumerator Post(string api, string json, Action<UnityWebRequest> Accept, Action Decline, bool debug = false)
    {
        if (debug)
        {
            Debug.Log(json);
        }
        //byte[] dataToPut = System.Text.Encoding.UTF8.GetBytes(j);
        UnityWebRequest www = UnityWebRequest.Put("http://" + IP + api, json);

        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        Reaction(www, Accept, Decline, debug);
    }

    public static IEnumerator Get(Api api, string json, Action<UnityWebRequest> Accept, Action Decline, bool debug = false)
    {
        if (debug)
        {
            Debug.Log(json);
        }
        UnityWebRequest www = UnityWebRequest.Get("http://" + IP + stringApi[(int)api] + json);

        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);

        Reaction(www, Accept, Decline, debug);
    }

    public static IEnumerator Get(string api, string json, Action<UnityWebRequest> Accept, Action Decline, bool debug = false)
    {
        if (debug)
        {
            Debug.Log(json);
        }
        UnityWebRequest www = UnityWebRequest.Get("http://" + IP + api + json);

        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);

        Reaction(www, Accept, Decline, debug);
    }

    public static IEnumerator DownloadImage(string url, RawImage im)
    {
        im.enabled = false;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            im.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            im.SetNativeSize();
        }
        im.enabled = true;
    }

    private static void Reaction(UnityWebRequest www, Action<UnityWebRequest> Accept, Action Decline, bool debug = false)
    {
        if (debug)
        {
            Debug.Log(www.url);
            Debug.Log(www.downloadHandler.text);
        }

        if (www.isNetworkError || www.isHttpError)
        {
            if (debug) Debug.Log(www.error);
            Decline();
        }
        else
        {
            if (debug) Debug.Log(www.downloadHandler.text);
            Accept(www);
        }
    }
}
