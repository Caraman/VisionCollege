using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;

namespace hangman
{
	public class SimpleListItem2Adapter : BaseAdapter<highscore>
	{
		List<highscore> items;

		Activity context;

	public SimpleListItem2Adapter (Activity context, List<highscore> items) : base()
	{
		this.context = context;
		this.items = items;
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	public override highscore this[int position]
	{
		get { return items[position]; }
	}

	public override int Count
	{
		get { return items.Count; }
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		var item = items[position];

		View view = convertView;

		if (view == null) 
		{
			view = context.LayoutInflater.Inflate (Android.Resource.Layout.SimpleListItem2, null);
		}

		TextView text1 = view.FindViewById<TextView> (Android.Resource.Id.Text1);
		text1.Text = item.score;
		text1.Gravity = GravityFlags.Center;

		TextView text2 = view.FindViewById<TextView> (Android.Resource.Id.Text2);
		text2.Text = item.username;
		text2.Gravity = GravityFlags.Center;

		return view;

		}
	}
}
