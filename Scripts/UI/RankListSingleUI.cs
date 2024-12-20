using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankListSingleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _playerScore;

    private RankData _rank;

    public void UpdateRank(RankData rank)
    {
        this._rank = rank;
        _playerNameText.text = _rank.name;
        _playerScore.text = _rank.score.ToString();
    }
}
