//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Linq;

//[Serializable]
//public class DialogSystemCanvas : DialogSystem
//{
//    protected override ChoiceRef SelectedChoice { get; set; }

//    public Animator anim;
//    public TMP_Text text_speaker, text_content;
//    public Image sprite_speaker;

//    public GameObject buttonsParent;

//    private List<Button> buttons = new();
//    private ChoiceRef[] choiceRefs;
//    private bool hasSetListeners;

//    protected override IEnumerator OnShow(GameObject anchor, ActorSO actor, string text, IEnumerable<SpriteDefinition> sprites, params ChoiceRef[] choices)
//    {
//        choiceRefs = choices.Where(x => x.IsVisible && x.IsBlocked == false).ToArray();

//        if (!hasSetListeners)
//        {
//            if (buttonsParent != null)
//            {
//                this.buttons = buttonsParent.GetComponentsInChildren<Button>().ToList();

//                for (int i = 0; i < this.buttons.Count; i++)
//                {
//                    var id = i;
//                    buttons[i].onClick.AddListener(() => this.ButtonPressed(id));
//                }

//                hasSetListeners = true;
//            }
//        }

//        anim.SetBool("visible", true);
//        text_speaker.text = actor?.Name;
//        sprite_speaker.sprite = actor?.Icon;

//        foreach (var button in buttons)
//        {
//            button.gameObject.SetActive(false);
//        }

//        for (int i = 0; i < buttons.Count; i++)
//        {
//            var choice = choiceRefs.Skip(i).FirstOrDefault();
//            if (choice != null)
//            {
//                buttons[i].gameObject.SetActive(choice != null);
//                buttons[i].GetComponentInChildren<TMP_Text>().text = choice?.Text;
//            }
//        }

//        text_content.text = text;

//        yield return null;
//    }

//    public void ButtonPressed(int id)
//    {
//        this.SelectedChoice = choiceRefs.Skip(id).FirstOrDefault();
//        this.ConfirmChoice();
//    }

//    protected override IEnumerator OnHide()
//    {
//        anim.SetBool("visible", false);

//        yield return null;
//    }
//}