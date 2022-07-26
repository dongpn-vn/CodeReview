using System;
using System.Threading;
using UnityEngine;
using Grpc.Core;
using Dongpn.Singleton;

public class AppManager : AllSceneSingleton<AppManager>
{
    private Channel _channel;
    public Channel Channel
    {
        get
        {
            if (_channel == null)
            {
                GetChannel();
            }
            return _channel;
        }
    }

    private CallOptions _options;
    public CallOptions Options
    {
        get
        {
            if (_options.Headers == null)
            {
                SetHeader();
            }
            return _options;
        }
    }

    public bool enableTLS = false;

    private Metadata _header = null;
    private DateTime? _deadline = null;
    private CancellationToken _cancellationToken = default;
    




    #region Unity_Function
    void Start()
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        if (_channel == null)
            GetChannel();
    }

    #endregion

    #region method
    private void GetChannel()
    {
        var credentials = new SslCredentials();
        var environmentIP = "";

        if (enableTLS)
        {
            _channel = new Channel(environmentIP, credentials);
        }
        else
        {
            _channel = new Channel(environmentIP, ChannelCredentials.Insecure);
        }

    }

    public void SetHeader()
    {
        _header = new Metadata
        {
            //{ "authorization", token},
            //{ "platform", platform},
            //{ "versionCode", versionCode},
        };

        _options = new CallOptions(_header, _deadline, _cancellationToken);
    }

    #endregion
}