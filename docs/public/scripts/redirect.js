const url = `${location.protocol}//${location.host}/lilToon/`;
var hash = location.hash.substring(1);
if(hash.length != 0)
{
    if(hash.startsWith('/ja-jp/') || hash.startsWith('/en-us/'))
    {
        hash = hash.substring(7);
    }
    const lang = window.navigator.language || window.navigator.userLanguage || window.navigator.browserLanguage;
    if(lang.startsWith('ja')) location.href = `${url}ja_JP/${hash}`;
    else location.href = `${url}ja_JP/${hash}`;
}