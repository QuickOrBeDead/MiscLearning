import { Quiz } from "./types";

const quiz: Quiz = {
    "title": "test",
    "questions": [
        {
            "text": "HOTSPOT -<br>You have developed a Web App for your company. The Web App provides services and must run in multiple regions.<br>You want to be notified whenever the Web App uses more than 85 percent of the available CPU cores over a 5 minute period. Your solution must minimize costs.<br>Which command should you use? To answer, select the appropriate settings in the answer area.<br>NOTE: Each correct selection is worth one point.",
            "answers": {
                "parts": [
                    {
                    "type": "Text",
                    "value": "az monitor metrics alert create -n myAlert -g myResourceGroup --scopes targetResourcelD --condition \""
                }, {
                    "type": "AnswerGroup",
                    "value": 0
                }, {
                    "type": "Text",
                    "value": "> 85\""
                }, {
                    "type": "AnswerGroup",
                    "value": 1
                }, {
                    "type": "Text",
                    "value": "5m\""
                }],
                "groups": [
                    {
                        "answers": [
                            {
                                "text": "CPU Usage",
                                "isCorrect": false
                            },
                            {
                                "text": "Percentage CPU",
                                "isCorrect": false
                            },
                            {
                                "text": "avg Percentage CPU",
                                "isCorrect": true
                            }
                        ]
                    },
                    {
                        "answers": [
                            {
                                "text": "--window-size",
                                "isCorrect": true
                            },
                            {
                                "text": "--evaluation-frequency",
                                "isCorrect": false
                            },
                            {
                                "text": "--auto-mitigate",
                                "isCorrect": false
                            }
                        ]
                    }
                ]
            },
            "questionType": "AnswerTemplate"
        }
    ]
}