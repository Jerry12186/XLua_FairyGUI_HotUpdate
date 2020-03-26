using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UpdateSys
{
	public class UpdateUi : MonoBehaviour
	{
		public static UpdateUi Instance;

		private Transform _downloadPanel;
		private Transform _bg;
		private Image _progressImage;
		private Text _downloadSpeedText;
		private Text _progressText;
		private Transform _text;

		private Transform _tipPanel;
		private Transform _wifiTip;
		private Transform _bg2;
		private Text _fileCountText;
		private Text _fileSizeText;
		private Button _okBtn;
		private Button _cancelBtn;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			_downloadPanel = transform.Find("DownloadPanel");
			_bg = _downloadPanel.Find("ProgressBg");
			_progressImage = _bg.Find("ProgressImage").GetComponent<Image>();
			_downloadSpeedText = _bg.Find("DownloadSpeedText").GetComponent<Text>();
			_progressText = _bg.Find("ProgressText").GetComponent<Text>();
			_text = _downloadPanel.Find("Text");

			_tipPanel = transform.Find("TipPanel");
			_bg2 = _tipPanel.Find("Bg");
			_wifiTip = _bg2.Find("WiFiTip");
			_fileCountText = _bg2.Find("FileCountText").GetComponent<Text>();
			_fileSizeText = _bg2.Find("FileSizeText").GetComponent<Text>();
			_okBtn = _bg2.Find("OkBtn").GetComponent<Button>();
			_cancelBtn = _bg2.Find("CancelBtn").GetComponent<Button>();

			InitUi();
		}

		private void InitUi()
		{
			_text.gameObject.SetActive(true);
			_bg.gameObject.SetActive(false);
			_okBtn.onClick.AddListener(OnOkBtnClick);
			_cancelBtn.onClick.AddListener(OnCancelClick);
		}

		private void OnOkBtnClick()
		{
			Debug.Log("确定按钮");
			_tipPanel.gameObject.SetActive(false);
			_downloadPanel.gameObject.SetActive(true);
			_text.gameObject.SetActive(false);
			_bg.gameObject.SetActive(true);
			
			FixFile.Instance.DownloadFile(() =>
			{
				Debug.Log("更新完成了，可以开始主程序了");
				StartGame();
			});
		}

		private void StartGame()
		{
			Initialization.CreateInitialization();
			//然后加载开始场景
		}
		private void OnCancelClick()
		{
			Debug.Log("取消按钮");
			Application.Quit();
		}

		public void SetDownloadInfo(float progress, float speed)
		{
			_progressImage.fillAmount = progress;
			_progressText.text = string.Format("{0}%", (int) (progress * 100));
			//汉字部分以后会涉及到本地化
			_downloadSpeedText.text = string.Format("下载速度:{0}KB/S", speed.ToString("0.0"));
		}

		public void SetDownloadFileInfo(int fileCount, double fileSize, bool bWifi = true)
		{
			_downloadPanel.gameObject.SetActive(false);
			_tipPanel.gameObject.SetActive(true);

			if (!bWifi)
			{
				_wifiTip.gameObject.SetActive(true);
			}
			else
			{
				_wifiTip.gameObject.SetActive(false);
			}

			//汉字记得本地化
			_fileCountText.text = string.Format("需要下载{0}个文件", fileCount);
			float size = (float)(fileSize / 1024 / 1024);
			_fileSizeText.text = string.Format("总共大小为{0}MB", size.ToString("0.00"));
		}
	}
}