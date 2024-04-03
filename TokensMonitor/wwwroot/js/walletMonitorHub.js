"use strict";

function initWalletMonitorHub(startConnectionCallback, onRefreshBalanceCallback, tokenProvider){
    const walletMonitor = new signalR.HubConnectionBuilder()
        .withUrl("/wallet-monitor", { accessTokenFactory: () =>{ return tokenProvider(); }})
        .build();
    
    walletMonitor.start().then(function () {
        if(startConnectionCallback && typeof startConnectionCallback === 'function'){
            startConnectionCallback();
        }
    }).catch(function (err) {
        return console.error(err.toString());
    });

    walletMonitor.on("RefreshBalance", function (response) {
        if(onRefreshBalanceCallback && typeof onRefreshBalanceCallback === 'function'){
            onRefreshBalanceCallback(response);
        }
    });
    
    return {
        start: function (address, addressList, periodMinutes, telegramApiKey, telegramChannelId, successCallback, errorCallback){
            const data = {
                address: address,
                addressList: addressList,
                periodMinutes: parseInt(periodMinutes),
                telegram: {
                    apiKey: telegramApiKey,
                    channelId: telegramChannelId
                }
            };
            walletMonitor.invoke("Start", data)
                .then(function () {
                    if(successCallback && typeof successCallback === 'function') {
                        successCallback();
                    }
                })
                .catch(function (err) {
                    if(errorCallback && typeof errorCallback === 'function') {
                        errorCallback();
                    }
                    return console.error(err.toString());    
                })
        },
        stop: function (successCallback, errorCallback){
            walletMonitor.invoke("Stop")
                .then(function () {
                    if(successCallback && typeof successCallback === 'function') {
                        successCallback();
                    }
                })
                .catch(function (err) {
                    if(errorCallback && typeof errorCallback === 'function') {
                        errorCallback();
                    }
                        return console.error(err.toString());
                });
        }
    };
}