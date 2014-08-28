namespace CriticalState.TweenSys
{
	#region Using
	using UnityEngine;
	using System.Collections.Generic;
	#endregion

	public class LeanTweenVisual : MonoBehaviour
	{
		#region Fields
		#region Public
		/// <summary>
		/// Structure containing the tween groups and items.
		/// Holds information to run sequential and concurrent tweens.
		/// </summary>
		public List<LeanTweenGroup> groupList = new List<LeanTweenGroup>();

		/// <summary>
		/// Indicates whether the entire tween should continue repeating.
		/// </summary>
		public bool repeat;

		/// <summary>
		/// Delay between repeats.
		/// </summary>
		public float repeatDelay;

		/// <summary>
		/// If it is repeating, time in game time to call next repeat.
		/// </summary>
		public float nextCall;

		/// <summary>
		/// Indicates if the whole tween should restart when object is
		/// enabled.
		/// </summary>
		public bool restartOnEnable;
		#endregion

		#region Private
		/// <summary>
		/// Variable indicating if the initial method has been
		/// called once.  This is because we don't want to start
		/// animating in Start, we want to animate in the first update.
		/// </summary>
		private bool _calledOnce;
		#endregion
		#endregion

		#region MonoBehaviour Methods
		/// <summary>
		/// Initialize variables.  This is put in start
		/// to be compatible with object recycler.
		/// </summary>
		private void Start()
		{
			_calledOnce = false;
		}

		/// <summary>
		/// Starts tween or calls again if repeat is turned on.
		/// </summary>
		private void Update()
		{
			if(!_calledOnce)
			{
				BuildTween(0);
				_calledOnce = true;
			}
			else if(repeat && nextCall < Time.time)
			{
				BuildTween(repeatDelay);
			}
		}


		/// <summary>
		/// Called on enable and if you want the tween
		/// to restart on enable / on active.
		/// </summary>
		private void OnEnable()
		{
			if(restartOnEnable)
			{
				BuildTween(0);
			}
		}

		/// <summary>
		/// Remove unnecessary tweens from LeanTween.
		/// </summary>
		private void OnDisable()
		{
			if(restartOnEnable)
			{
				LeanTween.cancel(gameObject);
			}
		}

		/// <summary>
		/// If object is destroyed, get rid of tweens.
		/// </summary>
		private void OnDestroy()
		{
			LeanTween.cancel(gameObject);
		}
		#endregion

		#region Public Methods
		public void CopyTo(LeanTweenVisual tween)
		{
			tween.nextCall = nextCall;
			tween.repeat = repeat;
			tween.repeatDelay = repeatDelay;
			tween.restartOnEnable = restartOnEnable;

			tween.groupList = new List<LeanTweenGroup>();
			foreach(LeanTweenGroup group in groupList)
			{
				tween.groupList.Add(new LeanTweenGroup(group));
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Builds the tween structure with all the appropriate values.
		/// Cancels all previous tweens to keep a clean tween list.
		/// The overallDelay variable is used to set a delay
		/// to the entire group.
		/// </summary>
		/// <param name="overallDelay">Overall delay.</param>
		private void BuildTween(float overallDelay)
		{
			LeanTween.cancel(gameObject);
			foreach(LeanTweenGroup group in groupList)
			{
				foreach(LeanTweenItem item in group.itemList)
				{
					LTDescr tween;
					float delay = item.delay + group.delay + overallDelay;
					if(item.action == TweenAction.ALPHA)
					{
						tween = LeanTween.alpha(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.ALPHA_VERTEX)
					{
						tween = LeanTween.alphaVertex(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.MOVE)
					{
						tween = LeanTween.move(gameObject, item.to, item.duration);
					}
					else if(item.action == TweenAction.MOVE_LOCAL)
					{
						tween = LeanTween.moveLocal(gameObject, item.to, item.duration);
					}
					else if(item.action == TweenAction.MOVE_LOCAL_X)
					{
						tween = LeanTween.moveLocalX(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.MOVE_LOCAL_Y)
					{
						tween = LeanTween.moveLocalY(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.MOVE_LOCAL_Z)
					{
						tween = LeanTween.moveLocalZ(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.MOVE_X)
					{
						tween = LeanTween.moveX(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.MOVE_Y)
					{
						tween = LeanTween.moveY(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.MOVE_Z)
					{
						tween = LeanTween.moveZ(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.MOVE_CURVED)
					{
						tween = LeanTween.move(gameObject, item.bezierPath.vec3, item.duration).setOrientToPath(item.orientToPath);
					}
					else if(item.action == TweenAction.MOVE_CURVED_LOCAL)
					{
						tween = LeanTween.moveLocal(gameObject, item.bezierPath.vec3, item.duration).setOrientToPath(item.orientToPath);
					}
					else if(item.action == TweenAction.ROTATE)
					{
						tween = LeanTween.rotate(gameObject, item.to, item.duration);
					}
					else if(item.action == TweenAction.ROTATE_AROUND)
					{
						tween = LeanTween.rotateAround(gameObject, item.axis, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.ROTATE_LOCAL)
					{
						tween = LeanTween.rotateLocal(gameObject, item.to, item.duration);
					}
					else if(item.action == TweenAction.ROTATE_X)
					{
						tween = LeanTween.rotateX(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.ROTATE_Y)
					{
						tween = LeanTween.rotateY(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.ROTATE_Z)
					{
						tween = LeanTween.rotateZ(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.SCALE)
					{
						tween = LeanTween.scale(gameObject, item.to, item.duration);
					}
					else if(item.action == TweenAction.SCALE_X)
					{
						tween = LeanTween.scaleX(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.SCALE_Y)
					{
						tween = LeanTween.scaleY(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.SCALE_Z)
					{
						tween = LeanTween.scaleZ(gameObject, item.to.x, item.duration);
					}
					else if(item.action == TweenAction.DELAYED_SOUND)
					{
						tween = LeanTween.delayedSound(item.audioClip, item.from, item.duration);
					}
					else
					{
						tween = null;
						Debug.Log("The tween '" + item.action.ToString() + " has not been implemented.");
						break;
					}

					tween = tween.setDelay(delay);
					tween = item.ease == LeanTweenType.animationCurve ? tween.setEase(item.animationCurve) : tween.setEase(item.ease);
					// Debug.Log("curve:"+item.animationCurve+" item.ease:"+item.ease);
					if(item.between == LeanTweenBetween.FromTo)
					{
						tween.setFrom(item.from);
					}

				}
				overallDelay += group.endTime;
			}

			nextCall = Time.time + overallDelay;
		}
		#endregion
	}

	/// <summary>
	/// Special lean tween actions used just for the mono GUI.
	/// Adds variables like From and FromTo and also puts them
	/// in a nice legible format for displaying in gui.
	/// </summary>
	public enum LeanTweenBetween{FromTo, To};

	/// <summary>
	/// Contains a single lean tween item.
	/// </summary>
	[System.Serializable]
	public class LeanTweenItem
	{
		/// <summary>
		/// Name of the tween, useful for documentation in
		/// the project.
		/// </summary>
		public string name = "Tween";

		/// <summary>
		/// The lean tween action to perform.
		/// </summary>
		public TweenAction action = TweenAction.MOVE;

		/// <summary>
		/// Indicates if the action if from a certain value to a certain value or
		/// from its current value to a value.
		/// </summary>
		public LeanTweenBetween between = LeanTweenBetween.To;

		/// <summary>
		/// The easing to use.
		/// </summary>
		public LeanTweenType ease = LeanTweenType.linear;
		public AnimationCurve animationCurve = AnimationCurve.Linear(0,0,1,1);

		/// <summary>
		/// The start value if provided.
		/// If it is propagated using a float value, it is just stored in the x value.
		/// </summary>
		public Vector3 from;

		/// <summary>
		/// The end vector value.
		/// If it is propagated using a float value, it is just stored in the x value.
		/// </summary>
		public Vector3 to;

		/// <summary>
		/// Axis to rotate around, useful only for rotate around tween.
		/// </summary>
		public Vector3 axis;

		/// <summary>
		/// The duration of the tween.
		/// </summary>
		public float duration = 1f;

		/// <summary>
		/// The delay of this tween based on the begining of the group.
		/// </summary>
		public float delay;

		/// <summary>
		/// Foldout used for GUI display.
		/// </summary>
		public bool foldout = true;

		/// <summary>
		/// Path used if the tween follows along one
		/// </summary>
		public LeanTweenPath bezierPath;

		public AudioClip audioClip;

		/// <summary>
		/// For use on path tweens
		/// </summary>
		public bool orientToPath;

		/// <summary>
		/// Instantiates LeanTweenGroup.
		/// </summary>
		public LeanTweenItem()
		{
		}

		public LeanTweenItem( float delay )
		{
			this.delay = delay;
		}
		
		/// <summary>
		/// Instantiates LeanTweenGroup by making a copy of group.
		/// </summary>
		/// <param name="group">Group.</param>
		public LeanTweenItem(LeanTweenItem item)
		{
			name = item.name;
			action = item.action;
			between = item.between;
			ease = item.ease;
			from = item.from;
			to = item.to;
			axis = item.axis;
			duration = item.duration;
			delay = item.delay;
			foldout = item.foldout;
		}
	}

	/// <summary>
	/// A single lean tween group that can have concurrently
	/// running lean tween items.
	/// </summary>
	[System.Serializable]
	public class LeanTweenGroup
	{
		/// <summary>
		/// Name of this group.
		/// </summary>
		public string name = "Tweens";

		/// <summary>
		/// The delay before tweens in this group start.
		/// </summary>
		public float delay = 0;

		/// <summary>
		/// Foldout used for GUI display.
		/// </summary>
		public bool foldout = true;

		/// <summary>
		/// List of concurrent tweens.
		/// </summary>
		public List<LeanTweenItem> itemList = new List<LeanTweenItem>();

		/// <summary>
		/// Instantiates LeanTweenGroup.
		/// </summary>
		public LeanTweenGroup()
		{
		}
		public LeanTweenGroup(float delay)
		{
			this.delay = delay;
		}

		/// <summary>
		/// Instantiates LeanTweenGroup by making a copy of group.
		/// </summary>
		/// <param name="group">Group.</param>
		public LeanTweenGroup(LeanTweenGroup group)
		{
			name = group.name;
			delay = group.delay;
			foldout = group.foldout;
			itemList.Clear();
			foreach(LeanTweenItem item in group.itemList)
			{
				itemList.Add(new LeanTweenItem(item));
			}
		}

		/// <summary>
		/// Gets the time in which the first tween will start
		/// including all delays.
		/// </summary>
		/// <value>The start time.</value>
		public float startTime
		{
			get
			{
				float min = float.MaxValue;
				foreach(LeanTweenItem item in itemList)
				{
					min = Mathf.Min(item.delay + delay, min);
				}
				if(itemList.Count == 0)
				{
					return 0;
				}
				else
				{
					return min;
				}
			}
		}

		/// <summary>
		/// Gets the time in which the last tween will finish
		/// including all delays and durations.
		/// </summary>
		/// <value>The end time.</value>
		public float endTime
		{
			get
			{
				float max = float.MinValue;
				foreach(LeanTweenItem item in itemList)
				{
					max = Mathf.Max(item.delay + item.duration + delay, max);
				}
				if(itemList.Count == 0)
				{
					return 0;
				}
				else
				{
					return max;
				}
			}
		}
	}
}