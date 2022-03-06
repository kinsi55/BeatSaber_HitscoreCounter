using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace HitscoreCounter {
	public class HitscoreCounter : BasicCustomCounter, INoteEventHandler, ISaberSwingRatingCounterDidFinishReceiver {
		TMP_Text[] hintLabels;
		TMP_Text[] valueLabels;
		int[] targets;
		int[] values;

		public override void CounterInit() {
			var label = CanvasUtility.CreateTextFromSettings(Settings);
			label.text = "Hitscores";
			label.fontSize = 3;

			TMP_Text CreateLabel(TextAlignmentOptions align, Vector3 offset, float fontSize = 2.69f) {
				var x = CanvasUtility.CreateTextFromSettings(Settings, offset);
				x.text = "0";
				x.alignment = align;
				x.fontSize = fontSize;

				return x;
			}

			var countersCount = Config.Instance.splits.Count;

			hintLabels = new TMP_Text[countersCount];
			valueLabels = new TMP_Text[countersCount];
			values = new int[countersCount];
			targets = new int[countersCount];

			var totalWidth = countersCount * 0.5f;
			var halfWidth = totalWidth / 2f;
			var labelStep = totalWidth / (countersCount - 1);

			if(Config.Instance.verticalLayout) {
				for(var i = countersCount; i-- != 0;) {
					hintLabels[i] = CreateLabel(TextAlignmentOptions.CaplineRight, new Vector3(-0.25f, -0.3f - (i * 0.25f)));
					valueLabels[i] = CreateLabel(TextAlignmentOptions.CaplineLeft, new Vector3(0.25f, -0.3f - (i * 0.25f)), 3f);
				}
			} else {
				if(countersCount == 1) {
					labelStep = 0;
					halfWidth = 0;
				}

				for(var i = countersCount; i-- != 0;) {
					hintLabels[i] = CreateLabel(TextAlignmentOptions.Center, new Vector3((i * labelStep) - halfWidth, -0.3f));
					valueLabels[i] = CreateLabel(TextAlignmentOptions.Center, new Vector3((i * labelStep) - halfWidth, -0.6f), 3f);
				}
			}

			var prevValue = ScoreModel.kMaxCutRawScore + 1;
			for(var i = 0; i < countersCount; i++) {
				if(!Config.Instance.verticalLayout)
					hintLabels[i].fontStyle = FontStyles.Underline;

				if(i == countersCount - 1 && Config.Instance.splits[i] == 0) {
					hintLabels[i].text = $"<{prevValue}";
					targets[i] = -1;
					break;
				}

				if(prevValue - Config.Instance.splits[i] > 1) {
					if(i == 0) {
						hintLabels[i].text = $">{Config.Instance.splits[i] - 1}";
					} else {
						hintLabels[i].text = $"{prevValue - 1}-{Config.Instance.splits[i]}";

						if(!Config.Instance.verticalLayout)
							hintLabels[i].fontSize = 2.2f;
					}
				} else {
					hintLabels[i].text = Config.Instance.splits[i].ToString();
				}
				prevValue = Config.Instance.splits[i];
				targets[i] = Config.Instance.splits[i] - 1;
			}
		}

		public override void CounterDestroy() {
			hintLabels = null;
			valueLabels = null;
			targets = null;
			values = null;
		}

		void IncrementValueLabelForHitscore(int hitscore) {
			for(var i = 0; i < targets.Length; i++) {
				if(hitscore > targets[i]) {
					valueLabels[i].text = (++values[i]).ToString();
					return;
				}
			}
		}

		private Dictionary<ISaberSwingRatingCounter, float> noteCutAccs = new Dictionary<ISaberSwingRatingCounter, float>();
		public void OnNoteCut(NoteData data, NoteCutInfo info) {
			if(!info.allIsOK || data.colorType == ColorType.None)
				return;

			noteCutAccs.Add(info.swingRatingCounter, info.cutDistanceToCenter);
			info.swingRatingCounter.RegisterDidFinishReceiver(this);
		}

		public void OnNoteMiss(NoteData data) { }

		public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter) {
			if(!noteCutAccs.TryGetValue(saberSwingRatingCounter, out var nci))
				return;

			noteCutAccs.Remove(saberSwingRatingCounter);

			ScoreModel.RawScoreWithoutMultiplier(saberSwingRatingCounter, nci, out int beforeCut, out int afterCut, out int cutDistance);

			IncrementValueLabelForHitscore(beforeCut + afterCut + cutDistance);
		}
	}
}
