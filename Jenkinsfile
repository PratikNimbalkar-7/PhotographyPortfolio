pipeline {
    agent any

    tools {
        jdk 'JDK'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                echo "No build needed for this project"
            }
        }

        stage('SonarQube Analysis') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat '''
"C:\\ProgramData\\Jenkins\\.jenkins\\tools\\hudson.plugins.sonar.SonarRunnerInstallation\\SonarScanner\\bin\\sonar-scanner.bat" ^
-Dsonar.projectKey=PhotographyPortfolio ^
-Dsonar.sources=. ^
-Dsonar.host.url=http://localhost:9000
'''
                }
            }
        }
    }
}
