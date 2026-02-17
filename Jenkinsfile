pipeline {

    agent any

    environment {
        SONAR_SCANNER_HOME = tool 'SonarScanner'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('SonarQube Analysis - Begin') {
            steps {
                withSonarQubeEnv('SonarServer') {
                    bat """
                    "%SONAR_SCANNER_HOME%\\SonarScanner.MSBuild.exe" begin /k:"MyDotNetProject" /d:sonar.login="%SONAR_AUTH_TOKEN%"
                    """
                }
            }
        }

        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test'
            }
        }

        stage('SonarQube Analysis - End') {
            steps {
                withSonarQubeEnv('SonarServer') {
                    bat """
                    "%SONAR_SCANNER_HOME%\\SonarScanner.MSBuild.exe" end /d:sonar.login="%SONAR_AUTH_TOKEN%"
                    """
                }
            }
        }

    }

    post {
        success {
            echo "Build + Sonar Successful for ${env.BRANCH_NAME}"
        }

        failure {
            echo "Build Failed for ${env.BRANCH_NAME}"
        }
    }
}
