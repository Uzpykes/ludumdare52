using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public RectTransform MainPanel;

    public Button Next;
    public Button Previous;
    public Button Close;

    int currentPage = 0;
    private RectTransform currentPageRect;
    public List<RectTransform> Pages;

    private void Start()
    {
        Next.onClick.AddListener(NextPage);
        Previous.onClick.AddListener(PreviousPage);
        Close.onClick.AddListener(CloseUI);
    }

    public void ShowUI()
    {
        currentPage = 0;
        MainPanel.gameObject.SetActive(true);
        Previous.interactable = false;
        if (Pages.Count <= 1)
            Next.interactable = false;
        Open(currentPage);
    }

    public void NextPage()
    {
        if (currentPage < Pages.Count - 1)
        {
            currentPage++;
        }
        Open(currentPage);
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
        }

        Open(currentPage);

    }

    public void CloseUI()
    {
        if (currentPageRect != null)
            currentPageRect.gameObject.SetActive(false);
        currentPage = 0;
        MainPanel.gameObject.SetActive(false);
    }

    public void Open(int index)
    {
        if (currentPageRect != null)
            currentPageRect.gameObject.SetActive(false);

        if (index > 0)
            Previous.interactable = true;
        else
            Previous.interactable = false;

        if (index < Pages.Count - 1)
            Next.interactable = true;
        else
            Next.interactable = false;

        currentPageRect = Pages[index];
        currentPageRect.gameObject.SetActive(true);
    }

}
