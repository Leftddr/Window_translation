import requests
import json
import sys

def main(src_lang, text):
    url = "https://kapi.kakao.com/v1/translation/translate"
    target_lang = ""
    if src_lang == "ko":
        src_lang = "kr"
        target_lang = "en"
    elif src_lang == "en":
        target_lang = "kr"
    queryString = {"query":text, "src_lang": src_lang, "target_lang": target_lang}
    header = {"Authorization" : "KakaoAK af35956434233a16d98f0c2170840ca3"}
    r = requests.get(url, headers = header, params = queryString)
    st_json = json.loads(r.text)
    #print("hello")
    print(st_json['translated_text'][0][0])

if __name__ == '__main__':
    sys_split = sys.argv[1].split(' ')
    text = ''
    for i in range(1, len(sys_split)):
        text += sys_split[i]
    main(sys_split[0], text)
