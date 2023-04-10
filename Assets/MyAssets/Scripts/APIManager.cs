using Firesplash.UnityAssets.SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour {
    private SocketIOCommunicator socket;
    private TrackManager trackManager;
    [SerializeField]
    private string baseUrl;

    public class DataTrack {
        public string id_mc;
        public string id_ma;
    }


    private void Awake() {
        socket = FindObjectOfType<SocketIOCommunicator>();
        trackManager = FindObjectOfType<TrackManager>();
    }

    private void Start() {
        LoadBaseUrl();
        ConnectSocket();
    }

    public string GetBaseUrl() {
        return baseUrl;
    }

    public void LoadBaseUrl() {
        string savedBaseUrl = PlayerPrefs.GetString(ConstantString.SAVED_BASE_URL);
        if (!string.IsNullOrEmpty(savedBaseUrl)) {
            baseUrl = savedBaseUrl;
        }
        string removedUrlProtocol = baseUrl;
        if (removedUrlProtocol.Contains("http://") || removedUrlProtocol.Contains("http:\\\\")) {
            removedUrlProtocol = removedUrlProtocol.Remove(0, 7);
            socket.secureConnection = false;
        } else if (removedUrlProtocol.Contains("https://") || removedUrlProtocol.Contains("https:\\\\")) {
            removedUrlProtocol = removedUrlProtocol.Remove(0, 8);
            socket.secureConnection = true;
        }
        socket.socketIOAddress = removedUrlProtocol;
    }

    public void SaveBaseUrl(string newBaseUrl) {
        PlayerPrefs.SetString(ConstantString.SAVED_BASE_URL, newBaseUrl);
        StartCoroutine(ReconnectSocket());
    }

    private IEnumerator ReconnectSocket() {
        DisconnectSocket();
        while (socket.Instance.IsConnected()) {
            yield return null;
        }
        LoadBaseUrl();
        ConnectSocket();
    }

    public void ConnectSocket() {
        socket.Instance.On("generateTask", (string payload) => {
            Debug.Log("Data received: " + payload);
            DataTrack dataTrack = JsonUtility.FromJson<DataTrack>(payload);
            if (!string.IsNullOrEmpty(dataTrack.id_mc) && !string.IsNullOrEmpty(dataTrack.id_ma)) {
                trackManager.OnTrackPlay(dataTrack.id_mc, dataTrack.id_ma);
            }
        }
        );
        Debug.Log("Connecting socket to " + socket.socketIOAddress);
        socket.Instance.Connect();
    }

    public void DisconnectSocket() {
        if (!socket.Instance.IsConnected())
            return;
        socket.Instance.Close();
    }

    public void FinishTask(string id_ma) {
        WWWForm form = new WWWForm();
        form.AddField("id_ma", id_ma);
        StartCoroutine(PostDataCoroutine(baseUrl, "api/v1/robots/callback", form));
    }

    private IEnumerator PostDataCoroutine(string rootUrl, string subUri, WWWForm form) {
        string rootUrlPost = rootUrl;
        string uri = string.Format("{0}/{1}", rootUrlPost, subUri);
        Debug.Log(uri);
        UnityWebRequest uwr = UnityWebRequest.Post(uri, form);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError) {
            Debug.Log(uwr.error);
        } else {
            Debug.Log(uwr.downloadHandler.text);
        }
    }
}
