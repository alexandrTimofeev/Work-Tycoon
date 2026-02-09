using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CreateSVButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private string idObject;
    [SerializeField] private CreateObjectScrollView scrollView;
    public TetraObjectData tetraObjectData => G.GlobalData.TetraObjectDatas[idObject];

    [Space]
    [SerializeField] private Image imageButton;
    [SerializeField] private Image imageIcon;
    [SerializeField] private InfoOver infoOver;
    [SerializeField] private Button buttonUpgrade;

    [Space]
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textCost;
    [SerializeField] private TextMeshProUGUI textCostUpgrade;
    [SerializeField] private TextMeshProUGUI textUp;

    [Space]
    [SerializeField] private GameObject lockGO;
    [SerializeField] private TextMeshProUGUI tmpLock;
    private bool isLock;

    private Button button;
    private int level = 1;

    private bool isInteract = true;
    public bool IsInteract
    {
        get { return isInteract && !isLock; }
        private set { isInteract = value; }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Init(string idObject, CreateObjectScrollView scrollView = null)
    {
        this.idObject = idObject;
        this.scrollView = scrollView;
        level = 1;
        UpdateInfoOver();

        if (!tetraObjectData.ConditionUnlock.IsZero)
            Lock();
    }

    private void UpdateInfoOver()
    {
        InfoOverData infoOverData = new InfoOverData(tetraObjectData.InfoOverData);
        if (tetraObjectData.Prefab.TryGetComponent(out TetraBehaviour tetraBehaviour))
        {
            infoOverData.Description += $"\n{tetraBehaviour.GetInfoBehaviourText(LevelForBehaviour)}\n";
        }
        infoOverData.Description += $"\nLevel: {level}";
        infoOverData.Name = tetraObjectData.Name;
        infoOver.OverrideInfoData(infoOverData);

        imageIcon.sprite = tetraObjectData.Icon;

        textName.text = tetraObjectData.Name;
    }

    public void SetSelected(bool selected)
    {
        // здесь можно менять цвет / анимацию
        button.interactable = !selected && IsInteract;
    }

    public GameObject CreateObject(Vector3 position)
    {
        GameObject ngo = GameG.CreateManager.CreateObject(tetraObjectData, position);
        GameG.CreateManager.ApplyCondition(new ApplyConditionInfo(tetraObjectData.Condition, LevelForCreate));

        if (ngo.TryGetComponent(out TetraBehaviour tetraBehaviour))
            tetraBehaviour.SetLevel(LevelForBehaviour);
        if (ngo.TryGetComponent(out TetraObject tetraObject))
            tetraObject.SetLevel(level);

        //gameObject.SetActive(false);
        scrollView.TestButton(this);

        return ngo;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        scrollView.Select(this);
        if (isLock)
            TryUnlock();
        else
            scrollView.Select(this);
    }

    public void SetInteractible(bool isInteractible)
    {
        IsInteract = isInteractible;
        imageButton.color = IsInteract ? Color.white : Color.blue;
        imageIcon.color = IsInteract ? tetraObjectData.IconColor : (Color.gray - new Color(0, 0, 0, 0.8f));
        UpdateTextInfo();

        if (IsInteract == false)
            SetSelected(false);
    }

    public int LevelForUpgrade => (int)(Mathf.Pow(5f, level - 1));
    public int LevelForCreate => (int)(Mathf.Pow(3f, level - 1));
    public int LevelForBehaviour => level;    

    public void Lock()
    {
        isLock = true;
        lockGO.SetActive(true);

        UpdateLockInfo();
    }

    public void UpdateLockInfo()
    {
        bool canUnlock = GameG.Exicuter.TestCondition(tetraObjectData.ConditionUnlock);
        string colorTag = canUnlock ? "" : "<color=red>";
        tmpLock.text = colorTag + tetraObjectData.ConditionUnlock.GetTextCondition(1);
    }

    public void TryUnlock()
    {
        if(TestUnlock())
        {
            GameG.CreateManager.ApplyCondition(new ApplyConditionInfo(tetraObjectData.ConditionUnlock, level));
            Unlock();
        }
    }

    public bool TestUnlock()
    {
        if (tetraObjectData.ConditionUnlock.TestCondition(GameExicuter.GetConditionContext()))
            return true;

        return false;
    }

    public void Unlock()
    {
        lockGO.SetActive(false);
        isLock = false;
        scrollView.TestButton(this);
    }

    public void TryUpgrade()
    {
        if (!tetraObjectData.ConditionUpgrade.TestCondition(GameExicuter.GetConditionContext(), LevelForUpgrade))
            return;

        GameG.CreateManager.ApplyCondition(new ApplyConditionInfo(tetraObjectData.ConditionUpgrade, LevelForUpgrade));

        level++;
        scrollView.TestButton(this);

        UpdateInfoOver();
        UpdateTextInfo();
        UpdateLockInfo();
    }

    private void UpdateTextInfo()
    {
        textCost.text = isLock ? "" : tetraObjectData.GetTextCost(LevelForCreate);
        textCostUpgrade.text = isLock ? "" : ("Upgrade:\n" + tetraObjectData.GetTextCostUpgrade(LevelForUpgrade));

        bool canUpgrade = isLock == false && GameG.Exicuter.TestCondition(tetraObjectData.ConditionUpgrade, LevelForUpgrade);
        buttonUpgrade.interactable = canUpgrade;
        textUp.color = canUpgrade ? Color.white : Color.gray;
        textCostUpgrade.color = canUpgrade ? Color.white : Color.gray;
    }
}