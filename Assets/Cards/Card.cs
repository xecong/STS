using UnityEngine;

public class Card
{
    public string CardName { get; private set; }

    public Card(string name)
    {
        CardName = name;
    }

    public virtual void PlayEffect()
    {
        Debug.Log($"🃏 {CardName} 카드가 사용됨!");
    }
}
