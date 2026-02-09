using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateObjectScrollView : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private Transform content;
    [SerializeField] private CreateSVButton buttonPref;

    private CreateSVButton currentButton;
    private IInput input;
    private CellPlacer cellPlacer;
    private List<CreateSVButton> buttons = new List<CreateSVButton>();

    public bool OnDrag => currentButton != null;

    public Action<CreateSVButton> OnCreateObject;
    public Action<CreateSVButton> OnStartDrag;
    public Action<CreateSVButton> OnEndDrag;

    public void Init(IInput input, CellPlacer cellPlacer)
    {
        this.input = input;
        //input.OnMoved += OnMoved;
        input.OnEnded += OnEnded;
        this.cellPlacer = cellPlacer;
        
        Debug.Log($"Init {G.GlobalData.TetraObjectDatas.Count}");

        InitButtons();
    }
    
    public void InitButtons()
    {
        Debug.Log($"InitButtons {G.GlobalData.TetraObjectDatas.Count}");
        foreach (var item in G.GlobalData.TetraObjectDatas.Values)
        {
            CreateButton(item);
        }
    }

    private void CreateButton(TetraObjectData item)
    {
        var nbutton = Instantiate(buttonPref, content);
        nbutton.Init(item.ID, this);

        buttons.Add(nbutton);
        TestButton(nbutton);
    }

    private void OnDestroy()
    {
        if (input == null) return;
        //input.OnMoved -= OnMoved;
        input.OnEnded -= OnEnded;
    }

    public void Select(CreateSVButton button)
    {
        if (currentButton == button || !button.IsInteract)
            return;

        UnSelectCurrent();

        currentButton = button;
        currentButton.SetSelected(true);

        OnStartDrag?.Invoke(currentButton);
    }

    public void UnSelect(CreateSVButton button)
    {
        if (currentButton != button)
            return;

        UnSelectCurrent();
    }

    private void UnSelectCurrent()
    {
        if (currentButton == null)
            return;

        currentButton.SetSelected(false);
        OnEndDrag?.Invoke(currentButton);
        currentButton = null;
    }

    private void OnEnded(Vector2 _)
    {
        UnSelectCurrent();
    }

    private void Update()
    {
        if (currentButton == null)
            return;

        if (input.GetOverGameObjectUI() != null)
            return;

        var ray = Camera.main.ScreenPointToRay(input.GetOverPosition());
        if (!Physics.Raycast(ray, out var hit))
            return;

        Vector2Int? cell = GameG.CellPlacer.WorldToCell(hit.point);
        if (cell == null || GameG.CellPlacer.IsCellValid(cell.Value) == false || 
            GameG.CellPlacer.CanPlaceObject(currentButton.tetraObjectData.Prefab.GetComponent<IGridObject>(), cell.Value) == false)
        {
            return;
        }

        GameObject ngo = currentButton.CreateObject(hit.point);
        UnSelectCurrent();

        if(ngo != null && ngo.TryGetComponent(out GridUniversalInteractable3D gridUniversal))
        {
            gridUniversal.InitGrid(cellPlacer);

            GameG.Player.BeganWork(G._Input.GetOverPosition());
            GameG.Player.TrySelect(gridUniversal.gameObject);
            GameG.Player.TryGetDrag(gridUniversal.gameObject, hit.point);
        }
    }
    
    public void TestAllButtons()
    {
        foreach (var button in buttons)
        {
            TestButton(button);
        }
    }

    public void TestButton(CreateSVButton createSVButton)
    {
        createSVButton.SetInteractible(TestCondition(createSVButton.tetraObjectData, createSVButton.LevelForCreate));
        createSVButton.UpdateLockInfo();
    }

    public static bool TestCondition(TetraObjectData tetraObjectData, int level)
    {
        return tetraObjectData.Condition.TestCondition(GameExicuter.GetConditionContext(), level);
    }
}