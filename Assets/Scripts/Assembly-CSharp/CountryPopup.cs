using System;
using System.Collections.Generic;
using Kiloo.Common.UI;
using TopRun;
using UnityEngine;

public class CountryPopup : UIScreen
{
	public class CountryListAdapter : ListAdapter<Country>
	{
		public GameObject countryCell;

		public List<Country> countryList;

		private CountryPopup countryPopup;

		public CountryListAdapter(List<Country> countryListT, GameObject cell, CountryPopup popup)
			: base(countryListT)
		{
			countryCell = cell;
			countryList = countryListT;
			countryPopup = popup;
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			if (container2 == null)
			{
				container2 = new Container(countryCell);
				CountryCellController component = container2.model.GetComponent<CountryCellController>();
				container2.tag = component;
				UIEventListener uIEventListener = UIEventListener.Get(container2.model);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(countryPopup.OnClickContainer));
				uIEventListener.parameter = container2;
			}
			CountryCellController countryCellController = (CountryCellController)container2.tag;
			countryCellController.countryName.text = GetItem(dataIndex).name;
			countryCellController.countryFlag.spriteName = GetItem(dataIndex).flag;
			countryCellController.countryLocale = GetItem(dataIndex).countryCode;
			IManager manager = TopRunRoot.manager;
			string text = GetItem(dataIndex).countryCode.ToLower();
			if (manager != null && text == manager.country.ToLower())
			{
				if (manager.tournament == null || text == manager.tournament.country.ToLower())
				{
					countryCellController.statusSprite.spriteName = "icon_character_selected";
					countryCellController.statusSprite.enabled = true;
					countryCellController.countryName.color = GlobalColors.COUNTRY_POPUP_SELECTED_CELL_TEXT_COLOUR;
				}
				else
				{
					countryCellController.statusSprite.spriteName = "icon_timer";
					countryCellController.statusSprite.enabled = true;
					countryCellController.countryName.color = GlobalColors.COUNTRY_POPUP_SELECTED_CELL_TEXT_COLOUR;
				}
			}
			else if (manager.tournament != null && text == manager.tournament.country.ToLower())
			{
				countryCellController.statusSprite.spriteName = "icon_character_owned";
				countryCellController.statusSprite.enabled = true;
				countryCellController.countryName.color = GlobalColors.COUNTRY_POPUO_NORMAL_CELL_TEXT_COLOUR;
			}
			else
			{
				countryCellController.statusSprite.enabled = false;
				countryCellController.countryName.color = GlobalColors.COUNTRY_POPUO_NORMAL_CELL_TEXT_COLOUR;
			}
			return container2;
		}
	}

	[SerializeField]
	private ScrollView countryListView;

	[SerializeField]
	private GameObject countryCell;

	[SerializeField]
	private UILabel popupTitle;

	[SerializeField]
	private UISprite currentCountryFlag;

	[SerializeField]
	private UILabel currentCountryName;

	[SerializeField]
	private UILabel infoText;

	private CountryListAdapter adapter;

	public override void Init()
	{
		base.Init();
		adapter = new CountryListAdapter(CountryManager.instance.getListOfCountries(), countryCell, this);
		countryListView.SetAdapter(adapter);
		popupTitle.color = GlobalColors.COUNTRY_POPUP_TITLE_COLOUR;
		currentCountryName.color = GlobalColors.COUNTRY_POPUP_SUBTITLE_COLOUR;
		infoText.color = GlobalColors.COUNTRY_POPUP_INFO_TEXT_COLOUR;
	}

	public override void Show()
	{
		base.Show();
		UpdatePopupUI();
		if (countryListView != null)
		{
			float offset = countryListView.rootPanel.baseClipRegion.w * 0.5f - countryListView.GetFirstVisibleContainerOnScreen().height * 0.5f;
			countryListView.SetSelection(CountryManager.instance.getCurrentCountryIndex(), ContainerPivot.Top, offset);
		}
	}

	private void OnDisable()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.tournamentChanged += TournamentChanged;
			manager.RefreshTournament();
		}
	}

	private void TournamentChanged(object sender, TournamentChangedEventArgs eventArg)
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null && eventArg.tournamentWasUpdated)
		{
			if (manager.tournament != null && manager.country != manager.tournament.country && manager.nextTournamentFetchTime < DateTime.UtcNow && (manager.score == 0 || manager.tournament.hasResult))
			{
				manager.FetchNextTournament();
			}
			manager.tournamentChanged -= TournamentChanged;
		}
	}

	public void OnClickContainer(GameObject go)
	{
		Container container = (Container)go.GetComponent<UIEventListener>().parameter;
		CountryCellController countryCellController = (CountryCellController)container.tag;
		TopRunRoot.manager.country = countryCellController.countryLocale.ToUpper();
		UpdatePopupUI();
		countryListView.SetSelection(container.dataIndex, ContainerPivot.Top, container.top - countryListView.rootPanel.transform.localPosition.y);
	}

	public void UpdatePopupUI()
	{
		currentCountryFlag.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
		currentCountryName.text = CountryManager.instance.GetCurrentTournamentCountry().name.ToUpper();
	}
}
