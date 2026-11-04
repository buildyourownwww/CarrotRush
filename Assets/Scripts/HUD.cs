using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HUD : MonoBehaviour
{
[SerializeField] Image[] hearts; // 3 heart images, left to right
[SerializeField] Sprite heartFull;
[SerializeField] Sprite heartEmpty;
[SerializeField] TMP_Text carrotText;

void Start()
{
SetHearts(3);
SetCarrots(0);
}
public void SetHearts(int hp)
{
for (int i = 0; i < hearts.Length; i++)
hearts[i].sprite = i < hp ? heartFull : heartEmpty;
}
public void SetCarrots(int count)
{
    carrotText.text = "x " + count;
}
}