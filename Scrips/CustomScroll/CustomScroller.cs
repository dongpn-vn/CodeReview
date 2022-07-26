using UnityEngine;
using UnityEngine.UI;
using System;

public class CustomScroller : MonoBehaviour
{
    [SerializeField] GameObject prefabs;
    [SerializeField] RectTransform contentsRect;
    [SerializeField] RectTransform viewPort;
    [SerializeField] ScrollRect scroller;

    public float spacing;
    public int numberObjects;
    private int numberItemCheck;


    private float prefabsLength;

    RectTransform[] objects;

    RectTransform tempRect;
    float pos;

    private int currentIndex = 0;
    private int totalItem;
    private int maxMoveIndex;

    private int left;
    private float leftNormalized;

    private float valueEachObject = 0;

    public Action<RectTransform, int> OnChangeItemIndex;
    public Action<RectTransform, int> OnInitItemObject;
    public Action<int> OnFinishChangeIndex;

    #region Unity Function
    private void Start()
    {
        prefabsLength = prefabs.GetComponent<RectTransform>().sizeDelta.x + spacing;
        if(objects == null || objects.Length == 0)
            objects = new RectTransform[numberObjects];

        if(numberObjects > 0)
        {
            numberItemCheck = numberObjects % 2 == 0 ? numberObjects / 2 : (numberObjects - 1) / 2; 
        }
    }

    private void OnEnable()
    {
        scroller.onValueChanged.AddListener(OnScrollChangeValue);
    }

    private void OnDisable()
    {
        scroller.onValueChanged.RemoveListener(OnScrollChangeValue);
    }


    #endregion

    #region Methods

    private void OnScrollChangeValue(Vector2 position)
    {
        if(valueEachObject <= 0)
        {
            return;
        }

        int temp = Mathf.FloorToInt((position.x-leftNormalized)/valueEachObject);

        if (leftNormalized > 0)
            temp++;

        if(currentIndex != temp)
        {
            if(Mathf.Abs(currentIndex-temp) >= numberObjects)
            {
                int startIndex = temp < numberItemCheck ? 0 : temp - numberItemCheck;
                int endIndex = startIndex + numberObjects < totalItem ? startIndex + numberObjects : totalItem - 1;

                int count = endIndex - startIndex;
                for(int i =0; i < count; i++)
                {
                    Vector3 local = objects[i].localPosition;
                    objects[i].localPosition = new Vector3((startIndex+i) * prefabsLength + left, local.y, local.z);

                    if (!objects[i].gameObject.activeSelf)
                        objects[i].gameObject.SetActive(true);

                    OnInitItemObject?.Invoke(objects[i], startIndex+i);
                }

                for (int i = count+1; i < numberObjects; i++)
                {
                    objects[i]?.gameObject.SetActive(false);
                }
            }
            else
            {
                bool flag = false;
                if(currentIndex < temp && temp > numberItemCheck)
                {
                    flag = true;
                }

                if (currentIndex > temp && temp < maxMoveIndex - numberItemCheck)
                {
                    flag = true;
                }

                if(flag)
                {
                    int count = Mathf.Abs(currentIndex - temp);
                    for (int i = 0; i < count; i++)
                    {
                        GetLastObjectAndPosition(currentIndex < temp, out tempRect, out pos);
                        if (pos >= 0 && pos < contentsRect.sizeDelta.x)
                        {
                            tempRect.localPosition = new Vector3(pos, tempRect.localPosition.y, tempRect.localPosition.z);
                        }
                    }
                }

               
            }

            currentIndex = temp;
            OnFinishChangeIndex?.Invoke(temp);
        }
    }

    public void InitScroller(int itemCount, int spacingValue = 0)
    {
        scroller.horizontalNormalizedPosition = 0f;
        left = spacingValue;
        totalItem = itemCount;
        float numberItemInView = viewPort.rect.width / prefabsLength;
        maxMoveIndex = Mathf.FloorToInt(itemCount - numberItemInView);
        valueEachObject = 1.0f / (itemCount - numberItemInView);

        if (objects == null || objects.Length == 0)
        {
            prefabsLength = prefabs.GetComponent<RectTransform>().sizeDelta.x + spacing;
            objects = new RectTransform[numberObjects];
        }


        int max = itemCount >= numberObjects ? numberObjects : itemCount;

        contentsRect.sizeDelta = new Vector2(itemCount*prefabsLength+ left, contentsRect.sizeDelta.y);

        leftNormalized = left / (contentsRect.sizeDelta.x - viewPort.rect.width);

        for (int i = 0; i < max; i++)
        {
            if(objects[i]==null)
            {
                GameObject go = GameObject.Instantiate(prefabs);
                objects[i] = go.GetComponent<RectTransform>();
                go.transform.SetParent(contentsRect, false);
            }

            Vector3 local = objects[i].localPosition;
            objects[i].localPosition = new Vector3(i * prefabsLength+ left, local.y, local.z);

            if (!objects[i].gameObject.activeSelf)
                objects[i].gameObject.SetActive(true);

            OnInitItemObject?.Invoke(objects[i],i);
        }

        for(int i = max; i < numberObjects; i++)
        {
            objects[i]?.gameObject.SetActive(false);
        }
    }

    void GetLastObjectAndPosition(bool isLeft, out RectTransform rt, out float pos)
    {
        //Debug.Log("spam?");
        RectTransform tempRect = objects[0];
        float tempPos = objects[0].localPosition.x;
        if (isLeft)
        {
            foreach (RectTransform rect in objects)
            {
                if (rect.localPosition.x < tempRect.localPosition.x)
                {
                    tempRect = rect;
                }

                if (rect.localPosition.x > tempPos)
                {
                    tempPos = rect.localPosition.x;
                }
            }

            tempPos += prefabsLength;
        }
        else
        {
            foreach (RectTransform rect in objects)
            {
                if (rect.localPosition.x > tempRect.localPosition.x)
                {
                    tempRect = rect;
                }

                if (rect.localPosition.x < tempPos)
                {
                    tempPos = rect.localPosition.x;
                }
            }

            tempPos -= prefabsLength;
        }

        rt = tempRect;
        pos = tempPos;

        int index = (int)(tempPos / prefabsLength);
        OnChangeItemIndex?.Invoke(tempRect, index);
    }

    public void MoveToItem(int index)
    {
        //Debug.Log("item index: " + index);
        //contentsRect.localPosition = new Vector3(-index * prefabsLength, contentsRect.localPosition.y, contentsRect.localPosition.z);
        scroller.horizontalNormalizedPosition = valueEachObject * index;
    }
    #endregion
}
