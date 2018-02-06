using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour {

    //生成角色
    int height = 125;
    int startpos = 725;
    int createcount = 0;
    public float speed = 2;
    ArrayList currentelements = new ArrayList();

    //管理器
    public float createInterval = 0.5f;
    float time = 0;
    float createtime = 0;
    float waittime = 0;
    bool begincreate = true;
    bool isslow = false;
    int score = 0;

    //声音
    AudioSource audiosource;
    AudioSource playeraudiosource;
    public AudioClip sfx_hit;
    public AudioClip sfx_create;
    public AudioClip sfx_end;
   
    //物件
    GameObject element;
    GameObject player;
    GameObject bg;
    Transform retryLayer;
    Text ScoreText;

    //难度相关
    int percent = 100;

    private void Awake()
    {
        audiosource = GetComponent<AudioSource>();
        playeraudiosource = transform.Find("/Canvas/player").GetComponent<AudioSource>();

        player = transform.Find("/Canvas/player").gameObject;
        element = transform.Find("/Canvas/elements/element").gameObject;
        bg = transform.Find("/Canvas/bglayer").gameObject;
        retryLayer = transform.Find("/Canvas/retryLayer");
        ScoreText = transform.Find("/Canvas/Score").GetComponent<Text>();

        ScoreText.text = "0";
        bg.GetComponent<bgmove>().setspeed(speed);
        retryLayer.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        checkclick();
        time += Time.deltaTime;
        if (time >= createInterval)
        {
            //统计生成时间
            createtime += createInterval;
            //减少误差
            time = createInterval - time;
            if (begincreate)
            {
                if (createcount == 0 || Random.Range(1, 100) > percent || isslow == true)
                {
                    createElement();
                    isslow = false;
                }
                else
                {
                    isslow = true;
                }
            }

            //如果生成到了最大数量，则停止生成
            if (createtime >= speed)
            {
                begincreate = false;
                waittime = speed / 8;
            }
        }

        //等待再次生成
        if (!begincreate && currentelements.Count <= 0)
        {
            waittime -= Time.deltaTime;
            if (waittime <= 0)
            {
                createtime = 0;
                createcount = 0;
                begincreate = true;
            }
        }
	}

    void createElement()
    {
        GameObject clone = Instantiate(element, element.transform.parent);

        clone.transform.position = new Vector3(startpos, height);
        clone.AddComponent<element1move>().setspeed(speed);

        clone.transform.localScale = new Vector3(1.5f, 1.5f);
        LeanTween.scale(clone, new Vector3(1, 1, 1), 0.25f).setOnComplete(()=>
        {
            LeanTween.scaleY(clone, 0.9f, 0.25f).setLoopPingPong();
        });

        createcount += 1;
        currentelements.Add(clone);
        playeraudiosource.PlayOneShot(sfx_create);
    }

    public void DeleteElement(GameObject clone)
    {
        currentelements.Remove(clone);
    }

    void checkclick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            int index = 0;
            foreach (GameObject obj in currentelements)
            {
                if (obj.transform.position.x <= 200 && obj.transform.position.x >= 75)
                {
                    player.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                    LeanTween.scale(player, new Vector3(1, 1, 1), 0.25f);

                    LeanTween.rotateZ(obj, Random.Range(540,720), 0.5f);
                    float size = Random.Range(1f, 2f);
                    LeanTween.scale(obj, new Vector3(size, size), 0.5f);
                    LeanTween.alpha(obj.GetComponent<RectTransform>(), 0, 0.5f);
                    LeanTween.move(obj, new Vector3(Random.Range(400, 800), Random.Range(540, -100), 0), 0.5f).setOnComplete(
                        ()=>
                        {
                            Destroy(obj);
                        });

                    //播放音效
                    playeraudiosource.PlayOneShot(sfx_hit);
                    if (currentelements.Count <= index+1)
                        playeraudiosource.PlayOneShot(sfx_end);

                    //删除物体
                    currentelements.Remove(obj);

                    //加分
                    AddScore(1);
                    break;
                }
                index++;
            }
        }
    }

    void AddScore(int num)
    {
        score += num;
        diffifultscore += num;
        ScoreText.text = score.ToString();
        ScoreText.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        LeanTween.scale(ScoreText.gameObject, new Vector3(1, 1, 1), 0.25f);
        AutoDifficulty();
    }

    int diffifultscore = 0;
    void AutoDifficulty()
    {
        percent = (int)(100 / Mathf.Pow(diffifultscore, 0.53f));
        if (percent <= 20)
        {
            percent = 100;
            diffifultscore = 0;
            createInterval = Mathf.Clamp(createInterval /= 2, 0.25f, 2);
        }
        Debug.LogFormat("percent: {0}; createInterval: {1}", percent, createInterval);
    }

    public void retry()
    {
        Time.timeScale = 0;
        retryLayer.gameObject.SetActive(true);
        retryLayer.transform.Find("retry").GetComponent<Button>().onClick.AddListener(reload);
    }

    void reload()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
