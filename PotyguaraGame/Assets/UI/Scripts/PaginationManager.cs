using UnityEngine;
using UnityEngine.UI;

public class PaginationManager : MonoBehaviour
{
    [SerializeField] private GameObject pageIndicatorPrefab;
    [SerializeField] private Sprite spriteNotSelected;
    [SerializeField] private Sprite spriteSelected;
    private int totalItens = 0;
    private Image[] indicators;

    public void SetPagination(int totalItens)
    {
        int childCount = transform.childCount;

        if (childCount > totalItens)
        {
            for (int i = childCount - 1; i >= totalItens; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        else if (childCount < totalItens)
        {
            for (int i = childCount; i < totalItens; i++)
            {
                GameObject obj = Instantiate(pageIndicatorPrefab, transform);
                obj.GetComponent<Image>().sprite = spriteNotSelected;
            }
        }

        this.totalItens = totalItens;
        indicators = new Image[totalItens];

        for (int i = 0; i < totalItens; i++)
            indicators[i] = transform.GetChild(i).GetComponent<Image>();

        UpdatePagination(0);
    }

    public void UpdatePagination(int itemIndex)
    {
        for (int i = 0; i < totalItens; i++)
        {
            indicators[i].sprite = (i == itemIndex) ? spriteSelected : spriteNotSelected;
        }
    }
}
